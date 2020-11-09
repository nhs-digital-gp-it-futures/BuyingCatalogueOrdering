using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
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
        [OrderingAutoData]
        public static void Validate_Create_AdditionalService_NullDeliveryDate_ReturnsNoErrors(
            [Frozen] Order order,
            [Frozen] CreateOrderItemModel model,
            OrderItemValidator validator)
        {
            model.DeliveryDate = null;
            var request = new CreateOrderItemAdditionalServiceRequest(order, model);

            var result = validator.Validate(request);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_AssociatedService_NullDeliveryDate_ReturnsNoErrors(
            [Frozen] Order order,
            [Frozen] CreateOrderItemModel model,
            OrderItemValidator validator)
        {
            model.DeliveryDate = null;
            var request = new CreateOrderItemAdditionalServiceRequest(order, model);

            var result = validator.Validate(request);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        [OrderingAutoData]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Frozen date required for successful validation")]
        public static void Validate_Create_Solution_AllValid_ReturnsNoErrors(
            [Frozen] DateTime date,
            CreateOrderItemSolutionRequest request,
            OrderItemValidator validator)
        {
            var result = validator.Validate(request);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_NullRequest_ThrowsException(OrderItemValidator validator)
        {
            Assert.Throws<ArgumentNullException>(() => _ = validator.Validate(null));
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_NullCommencementDateOnOrder_ThrowsException(
            [Frozen] Order order,
            CreateOrderItemSolutionRequest request,
            OrderItemValidator validator)
        {
            order.CommencementDate = null;

            Assert.Throws<ArgumentException>(() => _ = validator.Validate(request));
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_NullDeliveryDate_AddsErrorDetail(
            [Frozen] Order order,
            [Frozen] CreateOrderItemModel model,
            OrderItemValidator validator)
        {
            var expected = new ErrorDetails("DeliveryDateRequired", "DeliveryDate");
            model.DeliveryDate = null;

            var request = new CreateOrderItemSolutionRequest(order, model);

            var result = validator.Validate(request);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().Should().Be(expected);
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_TooLateDeliveryDateForSolution_AddsErrorDetail(
            [Frozen] Order order,
            [Frozen] CreateOrderItemModel model,
            [Frozen] ValidationSettings settings,
            OrderItemValidator validator)
        {
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");
            model.DeliveryDate = order.CommencementDate.Value.AddDays(settings.MaxDeliveryDateOffsetInDays + 1);

            var request = new CreateOrderItemSolutionRequest(order, model);

            var result = validator.Validate(request);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().Should().Be(expected);
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_TooEarlyDeliveryDateForSolution_AddsErrorDetail(
            [Frozen] Order order,
            [Frozen] CreateOrderItemModel model,
            OrderItemValidator validator)
        {
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");
            model.DeliveryDate = order.CommencementDate.Value.AddDays(-1);

            var request = new CreateOrderItemSolutionRequest(order, model);

            var result = validator.Validate(request);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Update_NullRequest_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentNullException>(() => context.Validator.Validate(null, CatalogueItemType.Solution, ProvisioningType.OnDemand));
        }

        [Test]
        public static void Validate_Update_NullProvisioningType_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentNullException>(() => context.Validator.Validate(
                context.UpdateRequestBuilder.Build(), CatalogueItemType.Solution, null));
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Update_NullCommencementDateOnOrder_ThrowsException(
            [Frozen] Order order,
            CatalogueItemType catalogueItemType,
            ProvisioningType provisioningType,
            UpdateOrderItemRequest request,
            OrderItemValidator validator)
        {
            order.CommencementDate = null;

            Assert.Throws<ArgumentException>(() => validator.Validate(request, catalogueItemType, provisioningType));
        }

        [Test]
        public static void Validate_Update_AllValid_ReturnsNoErrors()
        {
            var context = new OrderItemValidatorTestContext();
            var results = context.Validator.Validate(
                context.UpdateRequestBuilder.Build(),
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            results.Should().NotBeNull();
            results.Should().HaveCount(0);
        }

        [Test]
        public static void Validate_Update_TooLateDeliveryDateForSolution_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");

            var commencementDate = DateTime.Now;
            var deliveryDate = commencementDate.AddDays((context.Settings.MaxDeliveryDateWeekOffset * 7) + 1);

            var result = context.Validator.Validate(
                context.UpdateRequestBuilder.WithDeliveryDate(deliveryDate).Build(),
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Update_TooEarlyDeliveryDateForSolution_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");

            var commencementDate = DateTime.Now;
            var deliveryDate = commencementDate.AddDays(-1);

            var result = context.Validator.Validate(
                context.UpdateRequestBuilder.WithDeliveryDate(deliveryDate).Build(),
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Update_NullEstimationPeriod_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("EstimationPeriodRequiredIfVariableOnDemand", "EstimationPeriod");

            var result = context.Validator.Validate(
                context.UpdateRequestBuilder.WithEstimationPeriodName(null).Build(),
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Update_InvalidEstimationPeriod_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("EstimationPeriodValidValue", "EstimationPeriod");

            var result = context.Validator.Validate(
                context.UpdateRequestBuilder.WithEstimationPeriodName("Moose").Build(),
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        private sealed class OrderItemValidatorTestContext
        {
            public OrderItemValidatorTestContext()
            {
                Settings = new ValidationSettings { MaxDeliveryDateWeekOffset = 1 };
                Validator = new OrderItemValidator(Settings);
                OrderBuilder = OrderBuilder.Create().WithCommencementDate(DateTime.UtcNow);
                UpdateRequestBuilder = UpdateOrderItemRequestBuilder.Create()
                    .WithOrder(OrderBuilder.Build());
            }

            public OrderItemValidator Validator { get; }

            public ValidationSettings Settings { get; }

            public UpdateOrderItemRequestBuilder UpdateRequestBuilder { get; }

            public OrderBuilder OrderBuilder { get; }
        }
    }
}
