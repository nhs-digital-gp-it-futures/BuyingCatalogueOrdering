using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    public class OrderItemValidatorTests
    {
        [Test]
        public void Validate_NullSettings_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderItemValidator(null));
        }

        [Test]
        public void Validate_Create_AllValid_ReturnsNoErrors()
        {
            var context = new OrderItemValidatorTestContext();
            
            var result = context.Validator.Validate(context.CreateRequestBuilder.Build()).ToList();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public void Validate_Create_NullRequest_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentNullException>(() => context.Validator.Validate(null));
        }

        [Test]
        public void Validate_Create_NullCommencementDateOnOrder_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentException>(() =>
            context.Validator.Validate(context.CreateRequestBuilder.WithOrder(
                context.OrderBuilder.WithCommencementDate(null).Build()).Build()));
        }

        [Test]
        public void Validate_Create_InvalidProvisioningType_AddsErrorDetail()
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
        public void Validate_Create_InvalidCataloguePriceType_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("TypeValidValue", "Type");

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithCataloguePriceTypeName("Moose").Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public void Validate_Create_NullPriceOnFlat_AddsErrorDetail()
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
        public void Validate_Create_NullPriceOnTiered_HasNoErrors()
        {
            var context = new OrderItemValidatorTestContext();

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithCataloguePriceTypeName("Tiered").WithPrice(null).Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [Test]
        public void Validate_Create_InvalidCurrencyCode_AddsErrorDetail()
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
        public void Validate_Create_NullEstimationPeriod_AddsErrorDetail()
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
        public void Validate_Create_InvalidEstimationPeriod_AddsErrorDetail()
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
        public void Validate_Create_NullDeliveryDateForSolution_AddsErrorDetail()
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
        public void Validate_Create_TooLateDeliveryDateForSolution_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");
            
            var commencementDate = DateTime.Now;
            var deliveryDate = commencementDate.AddDays((context.Settings.MaxDeliveryDateMonthOffset * 7) + 1);

            var result = context.Validator.Validate(
                context.CreateRequestBuilder.WithOrder(
                    context.OrderBuilder.WithCommencementDate(commencementDate).Build()
                    ).WithDeliveryDate(deliveryDate).Build()).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public void Validate_Create_TooEarlyDeliveryDateForSolution_AddsErrorDetail()
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
        public void Validate_Update_NullRequest_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentNullException>(() => context.Validator.Validate(null, CatalogueItemType.Solution, ProvisioningType.OnDemand));
        }

        [Test]
        public void Validate_Update_NullCatalogueItemType_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentNullException>(() => context.Validator.Validate(
                context.UpdateRequestBuilder.Build(), null, ProvisioningType.OnDemand));
        }

        [Test]
        public void Validate_Update_NullProvisioningType_ThrowsException()
        {
            var context = new OrderItemValidatorTestContext();
            Assert.Throws<ArgumentNullException>(() => context.Validator.Validate(
                context.UpdateRequestBuilder.Build(), CatalogueItemType.Solution, null));
        }

        [Test]
        public void Validate_Update_AllValid_ReturnsNoErrors()
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
        public void Validate_Update_TooLateDeliveryDateForSolution_AddsErrorDetail()
        {
            var context = new OrderItemValidatorTestContext();
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");

            var commencementDate = DateTime.Now;
            var deliveryDate = commencementDate.AddDays((context.Settings.MaxDeliveryDateMonthOffset * 7) + 1);

            var result = context.Validator.Validate(
                context.UpdateRequestBuilder.WithDeliveryDate(deliveryDate).Build(),
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        public void Validate_Update_TooEarlyDeliveryDateForSolution_AddsErrorDetail()
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
        public void Validate_Update_NullEstimationPeriod_AddsErrorDetail()
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
        public void Validate_Update_InvalidEstimationPeriod_AddsErrorDetail()
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

        private class OrderItemValidatorTestContext
        {
            public OrderItemValidator Validator { get; }
            public ValidationSettings Settings { get; }
            public CreateOrderItemRequestBuilder CreateRequestBuilder { get; }
            public UpdateOrderItemRequestBuilder UpdateRequestBuilder { get; }
            public OrderBuilder OrderBuilder { get; }

            public OrderItemValidatorTestContext()
            {
                Settings = new ValidationSettings {MaxDeliveryDateMonthOffset = 1};
                Validator = new OrderItemValidator(Settings);
                OrderBuilder = OrderBuilder.Create().WithCommencementDate(DateTime.UtcNow);
                CreateRequestBuilder = CreateOrderItemRequestBuilder.Create()
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .WithCataloguePriceTypeName("Flat")
                    .WithProvisioningTypeName("OnDemand").WithOrder(OrderBuilder.Build());
                UpdateRequestBuilder = UpdateOrderItemRequestBuilder.Create()
                    .WithOrder(OrderBuilder.Build());
            }
        }
    }
}
