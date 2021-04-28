using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Services.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderServiceTests
    {
        [Test]
        public static void Constructor_NullAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderingPartyService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrder_ReturnsNull(
            CallOffId callOffId,
            OrderService service)
        {
            var result = await service.GetOrder(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrder_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var result = await service.GetOrder(order.CallOffId);

            Assert.NotNull(result);
            result.Should().Be(order);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderList_ReturnsNull(
            Guid organisationId,
            OrderService service)
        {
            var result = await service.GetOrders(organisationId);

            result.Should().BeEmpty();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderList_ReturnsOrderList(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var result = await service.GetOrders(order.OrderingParty.Id);

            Assert.NotNull(result);
            result.Count.Should().Be(1);
            result.First().Should().Be(order);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderSummary_ReturnsNull(
            CallOffId callOffId,
            OrderService service)
        {
            var result = await service.GetOrderSummary(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderSummary_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var result = await service.GetOrderSummary(order.CallOffId);

            Assert.NotNull(result);
            result.Should().Be(order);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderCompletedStatus_ReturnsNull(
            CallOffId callOffId,
            OrderService service)
        {
            var result = await service.GetOrderForStatusUpdate(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderCompletedStatus_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var result = await service.GetOrderForStatusUpdate(order.CallOffId);

            Assert.NotNull(result);
            result.Should().Be(order);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrder_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            string description,
            OrderingParty orderingParty,
            OrderService service)
        {
            context.OrderingParty.Add(orderingParty);
            await context.SaveChangesAsync();

            var order = await service.CreateOrder(description, orderingParty.Id);

            order.OrderingParty.Should().BeEquivalentTo(orderingParty);
            order.Description.Should().Be(order.Description);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrder_SavedToDb(
            [Frozen] ApplicationDbContext context,
            string description,
            OrderingParty orderingParty,
            OrderService service)
        {
            context.OrderingParty.Add(orderingParty);
            await context.SaveChangesAsync();

            var order = await service.CreateOrder(description, orderingParty.Id);

            var expectedOrder = context.Set<Order>().First(o => o.Equals(order));

            expectedOrder.Description.Should().Be(order.Description);
            expectedOrder.OrderingParty.Should().BeEquivalentTo(orderingParty);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void DeleteOrder_ThrowsArgumentNullException(
            OrderService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.DeleteOrder(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrder_UpdatesOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderService service)
        {
            order.IsDeleted.Should().BeFalse();

            context.Order.Add(order);
            await context.SaveChangesAsync();

            await service.DeleteOrder(order);

            order.IsDeleted.Should().BeTrue();
        }
    }
}
