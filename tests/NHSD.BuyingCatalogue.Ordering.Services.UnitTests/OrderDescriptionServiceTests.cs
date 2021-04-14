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
    internal static class OrderDescriptionServiceTests
    {
        [Test]
        public static void Constructor_NullHttpContextAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderDescriptionService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderDescriptionDoesNotExist(
            CallOffId callOffId,
            FundingSourceService service)
        {
            var result = await service.GetFundingSource(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderDescription_ReturnsOrderDescription(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderDescriptionService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expectedResult = order.Description;

            var result = await service.GetOrderDescription(order.CallOffId);

            Assert.NotNull(result);
            result.Should().Be(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_NullOrder_ThrowsException(
            string description,
            OrderDescriptionService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetOrderDescription(null, description));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_NullOrderDescription_ThrowsException(
            Order order,
            OrderDescriptionService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetOrderDescription(order, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesOrderDescription(
            Order order,
            string description,
            OrderDescriptionService service)
        {
            order.Description.Should().NotBe(description);

            await service.SetOrderDescription(order, description);

            order.Description.Should().Be(description);
        }
    }
}
