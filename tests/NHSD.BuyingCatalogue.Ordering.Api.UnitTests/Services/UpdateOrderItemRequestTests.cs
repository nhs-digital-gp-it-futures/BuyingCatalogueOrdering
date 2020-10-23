using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class UpdateOrderItemRequestTests
    {
        [Test]
        public static void Constructor_NullOrder_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new UpdateOrderItemRequest(
                null,
                null,
                null,
                0,
                null,
                null));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_DeliveryDate(
            DateTime deliveryDate,
            Order order)
        {
            var request = new UpdateOrderItemRequest(
                deliveryDate,
                null,
                order,
                0,
                null,
                null);

            request.DeliveryDate.Should().Be(deliveryDate);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_EstimationPeriod(
            TimeUnit estimationPeriod,
            Order order)
        {
            var request = new UpdateOrderItemRequest(
                null,
                estimationPeriod.Name(),
                order,
                0,
                null,
                null);

            request.EstimationPeriod.Should().Be(estimationPeriod);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Order(Order order)
        {
            var request = new UpdateOrderItemRequest(
                null,
                null,
                order,
                0,
                null,
                null);

            request.Order.Should().Be(order);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_OrderItemId(
            int orderItemId,
            Order order)
        {
            var request = new UpdateOrderItemRequest(
                null,
                null,
                order,
                orderItemId,
                null,
                null);

            request.OrderItemId.Should().Be(orderItemId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Price(
            decimal price,
            Order order)
        {
            var request = new UpdateOrderItemRequest(
                null,
                null,
                order,
                0,
                price,
                null);

            request.Price.Should().Be(price);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Quantity(
            int quantity,
            Order order)
        {
            var request = new UpdateOrderItemRequest(
                null,
                null,
                order,
                0,
                null,
                quantity);

            request.Quantity.Should().Be(quantity);
        }
    }
}
