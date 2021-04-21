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
    internal static class CommencementDateServiceTests
    {
        [Test]
        public static void Constructor_NullHttpContextAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CommencementDateService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetCommencementDate_ReturnsNull(
            CallOffId callOffId,
            CommencementDateService service)
        {
            var result = await service.GetCommencementDate(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetCommencementDate_ReturnsCommencementDate(
            [Frozen] ApplicationDbContext context,
            Order order,
            CommencementDateService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expectedResult = order.CommencementDate;

            var result = await service.GetCommencementDate(order.CallOffId);

            Assert.NotNull(result);
            result.Value.Should().Be(expectedResult!.Value);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetCommencementDate_NullOrder_ThrowsException(
            DateTime? commencementDate,
            CommencementDateService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetCommencementDate(null, commencementDate));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetCommencementDate_NullCommencementDate_ThrowsException(
            Order order,
            CommencementDateService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetCommencementDate(order, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task SetCommencementDate_UpdatesCommencementDate(
            Order order,
            DateTime commencementDate,
            CommencementDateService service)
        {
            order.CommencementDate.Should().NotBeSameDateAs(commencementDate);

            await service.SetCommencementDate(order, commencementDate);

            order.CommencementDate.Should().Be(commencementDate);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task SetCommencementDate_SavesToDb(
            [Frozen] ApplicationDbContext context,
            Order order,
            DateTime commencementDate,
            CommencementDateService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            order.CommencementDate.Should().NotBeSameDateAs(commencementDate);

            await service.SetCommencementDate(order, commencementDate);

            var expectedOrder = context.Set<Order>().First(o => o.Equals(order));

            expectedOrder.CommencementDate.Should().Be(commencementDate);
        }
    }
}
