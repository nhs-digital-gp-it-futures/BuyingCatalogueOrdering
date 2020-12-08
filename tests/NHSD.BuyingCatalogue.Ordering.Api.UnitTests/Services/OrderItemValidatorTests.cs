using System;
using System.Collections.Generic;
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
        [OrderingAutoData]
        public static void Validate_Create_Bulk_NullRequests_ThrowsException(
            IEnumerable<OrderItem> orderItems,
            OrderItemValidator validator)
        {
            Assert.Throws<ArgumentNullException>(() => validator.Validate(null, orderItems));
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_Bulk_NullOrderItems_ThrowsException(
            IReadOnlyList<CreateOrderItemRequest> requests,
            OrderItemValidator validator)
        {
            Assert.Throws<ArgumentNullException>(() => validator.Validate(requests, null));
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_Bulk_IdentifiesDuplicateOrderItemIds(
            CreateOrderItemModel model,
            Order order,
            OrderItem orderItem,
            Guid userId,
            string userName,
            OrderItemValidator validator)
        {
            var expectedFailedValidations = new Dictionary<int, ValidationResult>
            {
                { 1, new ValidationResult(new ErrorDetails(
                    nameof(UpdateOrderItemModel.OrderItemId) + "Duplicate",
                    nameof(UpdateOrderItemModel.OrderItemId)))
                },
            };

            model.DeliveryDate = order.CommencementDate;
            model.OrderItemId = orderItem.OrderItemId;
            order.AddOrderItem(orderItem, userId, userName);

            var requests = new[]
            {
                new CreateOrderItemSolutionRequest(order, model),
                new CreateOrderItemSolutionRequest(order, model),
            };

            var result = validator.Validate(requests, new[] { orderItem });

            result.Success.Should().BeFalse();
            result.FailedValidations.Should().BeEquivalentTo(expectedFailedValidations);
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_Bulk_AddsValidationErrorsWithExpectedIndex(
            CreateOrderItemModel model,
            Order order,
            OrderItem orderItem1,
            OrderItem orderItem2,
            OrderItemValidator validator)
        {
            var expectedFailedValidations = new Dictionary<int, ValidationResult>
            {
                { 0, new ValidationResult(new ErrorDetails(
                    nameof(UpdateOrderItemModel.DeliveryDate) + "OutsideDeliveryWindow",
                    nameof(UpdateOrderItemModel.DeliveryDate)))
                },
                { 1, new ValidationResult(new ErrorDetails(
                    nameof(UpdateOrderItemModel.DeliveryDate) + "Required",
                    nameof(UpdateOrderItemModel.DeliveryDate)))
                },
            };

            model.DeliveryDate = order.CommencementDate?.AddDays(-1);
            model.OrderItemId = orderItem1.OrderItemId;
            var request1 = new CreateOrderItemSolutionRequest(order, model);

            model.DeliveryDate = null;
            model.OrderItemId = orderItem2.OrderItemId;
            var request2 = new CreateOrderItemSolutionRequest(order, model);

            var requests = new[] { request1, request2 };

            var result = validator.Validate(requests, new[] { orderItem1, orderItem2 });

            result.Success.Should().BeFalse();
            result.FailedValidations.Should().BeEquivalentTo(expectedFailedValidations);
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_Bulk_IdentifiesNotFoundOrderItemId(
            CreateOrderItemModel model,
            Order order,
            OrderItem orderItem,
            OrderItemValidator validator)
        {
            var expectedFailedValidations = new Dictionary<int, ValidationResult>
            {
                { 0, new ValidationResult(new ErrorDetails(
                    nameof(UpdateOrderItemModel.OrderItemId) + "NotFound",
                    nameof(UpdateOrderItemModel.OrderItemId)))
                },
            };

            model.DeliveryDate = order.CommencementDate;
            model.OrderItemId = orderItem.OrderItemId + 1;

            var requests = new[] { new CreateOrderItemSolutionRequest(order, model) };

            var result = validator.Validate(requests, new[] { orderItem });

            result.Success.Should().BeFalse();
            result.FailedValidations.Should().BeEquivalentTo(expectedFailedValidations);
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Create_Bulk_MultipleOrdersWithZeroId_ReturnsNoErrors(
            CreateOrderItemModel model,
            Order order,
            OrderItem orderItem,
            OrderItemValidator validator)
        {
            model.DeliveryDate = order.CommencementDate;
            model.OrderItemId = 0;

            var request1 = new CreateOrderItemSolutionRequest(order, model);
            var request2 = new CreateOrderItemSolutionRequest(order, model);

            var requests = new[] { request1, request2 };

            var result = validator.Validate(requests, new[] { orderItem });

            result.Success.Should().BeTrue();
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
        [OrderingAutoData]
        public static void Validate_Update_NullRequest_ThrowsException(OrderItemValidator validator)
        {
            Assert.Throws<ArgumentNullException>(() => validator.Validate(null, CatalogueItemType.Solution, ProvisioningType.OnDemand));
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Update_NullProvisioningType_ThrowsException(
            UpdateOrderItemRequest request,
            OrderItemValidator validator)
        {
            Assert.Throws<ArgumentNullException>(() => validator.Validate(request, CatalogueItemType.Solution, null));
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
        [OrderingAutoData]
        public static void Validate_Update_AllValid_ReturnsNoErrors(
            Order order,
            UpdateOrderItemModel model,
            OrderItemValidator validator)
        {
            model.DeliveryDate = order.CommencementDate;
            var request = new UpdateOrderItemRequest(order, model);
            var results = validator.Validate(
                request,
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            results.Should().NotBeNull();
            results.Should().HaveCount(0);
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Update_TooLateDeliveryDateForSolution_AddsErrorDetail(
            [Frozen] ValidationSettings settings,
            Order order,
            UpdateOrderItemModel model,
            OrderItemValidator validator)
        {
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");

            var deliveryDate = order.CommencementDate?.AddDays(settings.MaxDeliveryDateOffsetInDays + 1);
            model.DeliveryDate = deliveryDate;

            var request = new UpdateOrderItemRequest(order, model);

            var result = validator.Validate(
                request,
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Update_TooEarlyDeliveryDateForSolution_AddsErrorDetail(
            Order order,
            UpdateOrderItemModel model,
            OrderItemValidator validator)
        {
            var expected = new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate");

            var deliveryDate = order.CommencementDate?.AddDays(-1);
            model.DeliveryDate = deliveryDate;

            var request = new UpdateOrderItemRequest(order, model);

            var result = validator.Validate(
                request,
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Update_NullEstimationPeriod_AddsErrorDetail(
            Order order,
            UpdateOrderItemModel model,
            OrderItemValidator validator)
        {
            var expected = new ErrorDetails("EstimationPeriodRequiredIfVariableOnDemand", "EstimationPeriod");

            model.DeliveryDate = order.CommencementDate;
            model.EstimationPeriod = null;
            var request = new UpdateOrderItemRequest(order, model);

            var result = validator.Validate(
                request,
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }

        [Test]
        [OrderingAutoData]
        public static void Validate_Update_InvalidEstimationPeriod_AddsErrorDetail(
            Order order,
            UpdateOrderItemModel model,
            OrderItemValidator validator)
        {
            var expected = new ErrorDetails("EstimationPeriodValidValue", "EstimationPeriod");

            model.DeliveryDate = order.CommencementDate;
            model.EstimationPeriod = "Moose";
            var request = new UpdateOrderItemRequest(order, model);

            var result = validator.Validate(
                request,
                CatalogueItemType.Solution,
                ProvisioningType.OnDemand).ToList();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be(expected);
        }
    }
}
