using System;
using System.Diagnostics.CodeAnalysis;
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
    internal static class OrderingPartyServiceTests
    {
        [Test]
        public static void Constructor_NullAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderingPartyService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderingParty_ReturnsNull(
            CallOffId callOffId,
            OrderingPartyService service)
        {
            var result = await service.GetOrder(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderingParty_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderingPartyService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expectedResult = order;

            var result = await service.GetOrder(order.CallOffId);

            Assert.NotNull(result);
            result.Should().Be(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderingParty_ReturnsOrderingParty(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderingPartyService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expectedResult = order.OrderingParty;

            var result = await service.GetOrder(order.CallOffId);

            Assert.NotNull(result);
            result.OrderingParty.Should().Be(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetOrderingParty_NullOrder_ThrowsException(
            OrderingParty orderingParty,
            Contact contact,
            OrderingPartyService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetOrderingParty(null, orderingParty, contact));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetOrderingParty_NullOrderingParty_ThrowsException(
            Order order,
            Contact contact,
            OrderingPartyService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetOrderingParty(order, null, contact));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetOrderingParty_NullContact_ThrowsException(
            Order order,
            OrderingParty orderingParty,
            OrderingPartyService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetOrderingParty(order, orderingParty, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task SetOrderingParty_UpdatesOrderingParty(
            Order order,
            OrderingParty orderingParty,
            Contact contact,
            OrderingPartyService service)
        {
            order.OrderingParty.Name.Should().NotBe(orderingParty.Name);
            order.OrderingParty.OdsCode.Should().NotBe(orderingParty.OdsCode);
            order.OrderingParty.Address.Should().NotBe(orderingParty.Address);
            order.OrderingPartyContact.Should().NotBe(contact);

            await service.SetOrderingParty(order, orderingParty, contact);

            order.OrderingParty.Name.Should().Be(orderingParty.Name);
            order.OrderingParty.OdsCode.Should().Be(orderingParty.OdsCode);
            order.OrderingParty.Address.Should().Be(orderingParty.Address);
            order.OrderingPartyContact.Should().Be(contact);
        }
    }
}
