using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
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
        [OrderingAutoData]
        public static void Constructor_NullModelQuantity_ThrowsException(
            Order order,
            CreateOrderItemModel model)
        {
            model.Quantity = null;

            Assert.Throws<ArgumentException>(() => _ = new UpdateOrderItemRequest(order, model));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_NullModelPrice_ThrowsException(
            Order order,
            CreateOrderItemModel model)
        {
            model.Price = null;

            Assert.Throws<ArgumentException>(() => _ = new UpdateOrderItemRequest(order, model));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_NullOrder_ThrowsException(UpdateOrderItemModel model)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new UpdateOrderItemRequest(null, model));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_DeliveryDate(
            [Frozen] DateTime deliveryDate,
            Order order,
            UpdateOrderItemModel model)
        {
            var request = new UpdateOrderItemRequest(order, model);

            request.DeliveryDate.Should().Be(deliveryDate);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_EstimationPeriod(
            [Frozen] TimeUnit estimationPeriod,
            Order order,
            UpdateOrderItemModel model)
        {
            var request = new UpdateOrderItemRequest(order, model);

            request.EstimationPeriod.Should().Be(estimationPeriod);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Order(
            Order order,
            UpdateOrderItemModel model)
        {
            var request = new UpdateOrderItemRequest(order, model);

            request.Order.Should().Be(order);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_OrderItemId(
            int orderItemId,
            Order order,
            UpdateOrderItemModel model)
        {
            model.OrderItemId = orderItemId;
            var request = new UpdateOrderItemRequest(order, model);

            request.OrderItemId.Should().Be(orderItemId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Price(
            [Frozen] decimal price,
            Order order,
            UpdateOrderItemModel model)
        {
            var request = new UpdateOrderItemRequest(order, model);

            request.Price.Should().Be(price);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Quantity(
            int quantity,
            Order order,
            UpdateOrderItemModel model)
        {
            model.Quantity = quantity;

            var request = new UpdateOrderItemRequest(order, model);

            request.Quantity.Should().Be(quantity);
        }
    }
}
