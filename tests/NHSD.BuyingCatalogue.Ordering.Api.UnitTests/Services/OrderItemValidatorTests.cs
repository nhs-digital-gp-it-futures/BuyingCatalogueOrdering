﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EnumsNET;
using FluentAssertions;
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
        public static void Contructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderItemValidator).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        public static void Validate_Create_AllValid_ReturnsNoErrors()
        {
            var context = new OrderItemValidatorTestContext();

            var result = context.Validator.Validate(context.CreateRequestBuilder.Build()).ToList();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public static void Validate_Create_NullRequest_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentNullException>(() => _ = context.Validator.Validate(null).ToList());
        }

        [Test]
        public static void Validate_Create_NullCommencementDateOnOrder_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            var order = context.OrderBuilder.WithCommencementDate(null).Build();
            var request = context.CreateRequestBuilder.WithOrder(order).Build();

            Assert.Throws<ArgumentException>(() => _ = context.Validator.Validate(request).ToList());
        }

        [Test]
        public static void Validate_Create_InvalidProvisioningType_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("ProvisioningTypeValidValue", "ProvisioningType");

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithProvisioningTypeName("Moose").Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Create_InvalidCataloguePriceType_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("TypeValidValue", "Type");

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithCataloguePriceTypeName("Moose").Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [TestCase]
        public static void Validate_Create_NullEstimationPeriodOnProvisioningTypeDeclarative_HasNoErrors()
        {
            var context = new OrderItemValidatorTestContext();
            var request = context.CreateRequestBuilder
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .WithEstimationPeriodName(null)
                .WithProvisioningTypeName(ProvisioningType.Declarative.GetName())
                .Build();

            var result = context.Validator.Validate(request).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [TestCase]
        public static void Validate_Create_NullEstimationPeriodOnProvisioningTypePatient_HasNoErrors()
        {
            var context = new OrderItemValidatorTestContext();
            var request = context.CreateRequestBuilder
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .WithEstimationPeriodName(null)
                .WithProvisioningTypeName(ProvisioningType.Patient.GetName())
                .Build();

            var result = context.Validator.Validate(request).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [Test]
        public static void Validate_Create_NullPriceOnFlat_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("PriceRequired", "Price");

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithPrice(null).Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Create_NullPriceOnTiered_HasNoErrors()
        {
            var context = new OrderItemValidatorTestContext();

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithCataloguePriceTypeName("Tiered").WithPrice(null).Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [Test]
        public static void Validate_Create_InvalidCurrencyCode_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("CurrencyCodeValidValue", "CurrencyCode");

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithCurrencyCode("Moose").Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Create_NullEstimationPeriod_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("EstimationPeriodRequiredIfVariableOnDemand", "EstimationPeriod");

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithEstimationPeriodName(null).Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Create_InvalidEstimationPeriod_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("EstimationPeriodValidValue", "EstimationPeriod");

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithEstimationPeriodName("Moose").Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Create_NullDeliveryDateForSolution_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("DeliveryDateRequired", "DeliveryDate");

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithDeliveryDate(null).Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Create_TooLateDeliveryDateForSolution_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");

            var commencementDate = DateTime.Now;
            var deliveryDate = commencementDate.AddDays((context.Settings.MaxDeliveryDateWeekOffset * 7) + 1);

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithOrder(
                    context.OrderBuilder.WithCommencementDate(commencementDate).Build()
                    ).WithDeliveryDate(deliveryDate).Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Create_TooEarlyDeliveryDateForSolution_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");

            var commencementDate = DateTime.Now;
            var deliveryDate = commencementDate.AddDays(-1);

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithDeliveryDate(deliveryDate).Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public static void Validate_Update_NullRequest_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentNullException>(() => context.Validator.Validate(null, CatalogueItemType.Solution, ProvisioningType.OnDemand));
        }

        [Test]
        public static void Validate_Update_NullCatalogueItemType_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentNullException>(() => context.Validator.Validate(
                context.UpdateRequestBuilder.Build(), null, ProvisioningType.OnDemand));
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
                CreateRequestBuilder = CreateOrderItemRequestBuilder.Create()
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .WithCataloguePriceTypeName("Flat")
                    .WithProvisioningTypeName("OnDemand")
                    .WithOrder(OrderBuilder.Build());
                UpdateRequestBuilder = UpdateOrderItemRequestBuilder.Create()
                    .WithOrder(OrderBuilder.Build());
            }

            public OrderItemValidator Validator { get; }

            public ValidationSettings Settings { get; }

            public CreateOrderItemRequestBuilder CreateRequestBuilder { get; }

            public UpdateOrderItemRequestBuilder UpdateRequestBuilder { get; }

            public OrderBuilder OrderBuilder { get; }
        }
    }
}
