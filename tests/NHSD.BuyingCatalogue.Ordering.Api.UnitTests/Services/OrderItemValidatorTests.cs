using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderItemValidatorTests
    {
        [Test]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderItemValidator).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Constructor_NullValidationSettings_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderItemValidator(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Validate_NullCommencementDate_ThrowsArgumentNullException(
            Order order,
            CreateOrderItemModel model,
            CatalogueItemType itemType,
            OrderItemValidator orderItemValidator)
        {
            order.CommencementDate = null;

            Assert.Throws<ArgumentNullException>(() => _ = orderItemValidator.Validate(order, model, itemType));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Validate_AssociatedService_SuccessIsTrue(
            Order order,
            CreateOrderItemModel model,
            OrderItemValidator orderItemValidator)
        {
            model.ServiceRecipients.Should().NotBeNullOrEmpty();
            order.CommencementDate.Should().NotBeNull();

            var result = orderItemValidator.Validate(order, model, CatalogueItemType.AssociatedService);

            result.FailedValidations.Should().BeEmpty();
            result.Success.Should().BeTrue();
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Validate_NullDeliveryDate_SuccessIsFalse(
            Order order,
            CreateOrderItemModel model,
            OrderItemValidator orderItemValidator)
        {
            model.ServiceRecipients.Should().NotBeNullOrEmpty();
            order.CommencementDate.Should().NotBeNull();

            var serviceRecipients = model.ServiceRecipients.Select(_ =>
                new OrderItemRecipientModel { DeliveryDate = null }).ToList();

            var result = orderItemValidator.Validate(order, new CreateOrderItemModel { ServiceRecipients = serviceRecipients }, CatalogueItemType.AdditionalService);

            result.FailedValidations.Should().NotBeEmpty();

            foreach ((_, ValidationResult validationResult) in result.FailedValidations)
            {
                var errorDetails = validationResult.Errors.SingleOrDefault();
                errorDetails.Should().NotBeNull();
                errorDetails.Field.Should().Be("DeliveryDate");
                errorDetails.Id.Should().Be("DeliveryDateRequired");
            }

            result.Success.Should().BeFalse();
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Validate_InvalidDeliveryDate_SuccessIsFalse(
            Order order,
            CreateOrderItemModel model,
            CatalogueItemType itemType,
            OrderItemValidator orderItemValidator)
        {
            model.ServiceRecipients.Should().NotBeNullOrEmpty();

            var serviceRecipients = model.ServiceRecipients.Select(_ =>
                new OrderItemRecipientModel { DeliveryDate = order.CommencementDate.Value.AddDays(-1) }).ToList();

            var result = orderItemValidator.Validate(order, new CreateOrderItemModel { ServiceRecipients = serviceRecipients }, itemType);

            result.FailedValidations.Should().NotBeEmpty();

            foreach ((_, ValidationResult validationResult) in result.FailedValidations)
            {
                var errorDetails = validationResult.Errors.SingleOrDefault();
                errorDetails.Should().NotBeNull();
                errorDetails.Field.Should().Be("DeliveryDate");
                errorDetails.Id.Should().Be("DeliveryDateOutsideDeliveryWindow");
            }

            result.Success.Should().BeFalse();
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Validate_DeliveryDateAfterMaxDeliveryDateOffsetInDays_SuccessIsFalse(
            Order order,
            CreateOrderItemModel model,
            CatalogueItemType itemType,
            ValidationSettings validationSettings)
        {
            model.ServiceRecipients.Should().NotBeNullOrEmpty();

            OrderItemValidator orderItemValidator = new OrderItemValidator(validationSettings);

            var serviceRecipients = model.ServiceRecipients.Select(_ =>
                new OrderItemRecipientModel { DeliveryDate = order.CommencementDate.Value.AddDays(validationSettings.MaxDeliveryDateOffsetInDays + 1) }).ToList();

            var result = orderItemValidator.Validate(order, new CreateOrderItemModel { ServiceRecipients = serviceRecipients }, itemType);

            result.FailedValidations.Should().NotBeEmpty();

            foreach ((_, ValidationResult validationResult) in result.FailedValidations)
            {
                var errorDetails = validationResult.Errors.SingleOrDefault();
                errorDetails.Should().NotBeNull();
                errorDetails.Field.Should().Be("DeliveryDate");
                errorDetails.Id.Should().Be("DeliveryDateOutsideDeliveryWindow");
            }

            result.Success.Should().BeFalse();
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Validate_DeliveryDateWithinMaxDeliveryDateOffsetInDays_SuccessIsTrue(
            Order order,
            CreateOrderItemModel model,
            CatalogueItemType itemType,
            ValidationSettings validationSettings)
        {
            model.ServiceRecipients.Should().NotBeNullOrEmpty();

            itemType.Should().NotBeEquivalentTo(CatalogueItemType.AssociatedService);

            var serviceRecipients = model.ServiceRecipients.Select(_ =>
                new OrderItemRecipientModel { DeliveryDate = order.CommencementDate.Value.AddDays(validationSettings.MaxDeliveryDateOffsetInDays - 1) }).ToList();

            OrderItemValidator orderItemValidator = new OrderItemValidator(validationSettings);
            order.CommencementDate.Should().NotBeNull();

            var result = orderItemValidator.Validate(order, new CreateOrderItemModel { ServiceRecipients = serviceRecipients }, itemType);

            result.FailedValidations.Should().BeEmpty();
            result.Success.Should().BeTrue();
        }
    }
}
