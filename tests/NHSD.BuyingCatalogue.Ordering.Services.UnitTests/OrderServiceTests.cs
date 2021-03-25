using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NHSD.BuyingCatalogue.Ordering.Services;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Services.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderServiceTests
    {
        [Test]
        public static void Constructor_NullHttpContextAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetCommencementDate_ReturnsCommencementDate(
            [Frozen] ApplicationDbContext context,
            Order order1,
            Order order2,
            OrderService service)
        {
            context.Order.AddRange(order1, order2);
            await context.SaveChangesAsync();

            var expectedResult = order2.CommencementDate;

            var result = await service.GetCommencementDate(order2.CallOffId);

            result.Value.Should().Be(expectedResult.Value);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_NullModel_ThrowsException(
            Order order,
            DateTime? commencementDate,
            OrderService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetCommencementDate(order, null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetCommencementDate(null, commencementDate));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesCommencementDate(
            Order order,
            DateTime? commencementDate,
            OrderService service)
        {
            order.CommencementDate.Should().NotBeSameDateAs(commencementDate.GetValueOrDefault());

            await service.SetCommencementDate(order, commencementDate);

            order.CommencementDate.Should().Be(commencementDate);
        }
    }
}
