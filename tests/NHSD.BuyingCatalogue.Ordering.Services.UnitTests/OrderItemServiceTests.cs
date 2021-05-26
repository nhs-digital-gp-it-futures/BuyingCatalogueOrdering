using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Services.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderItemServiceTests
    {
        [Test]
        public static void Constructor_NullAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderItemService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrder_ReturnsNull(
            CallOffId callOffId,
            OrderItemService service)
        {
            var result = await service.GetOrder(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrder_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderItemService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var result = await service.GetOrder(order.CallOffId);

            Assert.NotNull(result);
            result.Should().Be(order);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderWithCatalogueItems_ReturnsNull(
            CallOffId callOffId,
            OrderItemService service)
        {
            var result = await service.GetOrderWithCatalogueItems(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderWithCatalogueItems_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            List<OrderItem> orderItems,
            OrderItemService service)
        {
            context.Order.Add(order);
            orderItems.ForEach(oi => order.AddOrUpdateOrderItem(oi));
            await context.SaveChangesAsync();

            var result = await service.GetOrderWithCatalogueItems(order.CallOffId);

            Assert.NotNull(result);
            result.Should().Be(order);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderItem_ReturnsNull(
            CallOffId callOffId,
            OrderItemService service)
        {
            var result = await service.GetOrderWithCatalogueItems(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderItem_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderItem orderItem,
            OrderItemService service)
        {
            context.Order.Add(order);
            order.AddOrUpdateOrderItem(orderItem);
            await context.SaveChangesAsync();

            var result = await service.GetOrderWithCatalogueItems(order.CallOffId);

            Assert.NotNull(result);
            result.Should().Be(order);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderItems_InvalidCallOffId_ReturnsNull(
            [Frozen] ApplicationDbContext context,
            CallOffId callOffId,
            CatalogueItemType catalogueItemType,
            OrderItemService service)
        {
            (await context.Order.AnyAsync(o => o.CallOffId == callOffId)).Should().BeFalse();
            var result = await service.GetOrderItems(callOffId, catalogueItemType);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderItems_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderItem orderItem,
            OrderItemService service)
        {
            context.Order.Add(order);
            order.AddOrUpdateOrderItem(orderItem);
            await context.SaveChangesAsync();

            var result = await service.GetOrderItems(order.CallOffId, order.OrderItems[0].CatalogueItem.CatalogueItemType);

            Assert.NotNull(result);
            result.Should().BeEquivalentTo(orderItem);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void DeleteOrderItem_NullOrder_ThrowsException(
            CatalogueItemId catalogueItemId,
            OrderItemService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _ = service.DeleteOrderItem(null, catalogueItemId));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItem_DeletesOrderItem(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderItem orderItem,
            OrderItemService service)
        {
            context.Order.Add(order);
            order.AddOrUpdateOrderItem(orderItem);
            await context.SaveChangesAsync();

            var result = await service.DeleteOrderItem(order, order.OrderItems[0].CatalogueItem.Id);

            Assert.NotNull(result);
            result.Should().Be(1);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItem_SavedToDb(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderItem orderItem,
            OrderItemService service)
        {
            order.OrderItems.Should().BeEmpty();
            context.Order.Add(order);
            order.AddOrUpdateOrderItem(orderItem);
            await context.SaveChangesAsync();

            await service.DeleteOrderItem(order, order.OrderItems[0].CatalogueItem.Id);

            var expectedOrder = context.Set<Order>().First(o => o.Equals(order));

            expectedOrder.OrderItems.Should().BeEmpty();
        }
    }
}
