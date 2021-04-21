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
    internal static class FundingSourceServiceTests
    {
        [Test]
        public static void Constructor_NullHttpContextAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new FundingSourceService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetFundingSource_ReturnsNull(
            CallOffId callOffId,
            FundingSourceService service)
        {
            var result = await service.GetFundingSource(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetFundingSource_ReturnsFundingSource(
            [Frozen] ApplicationDbContext context,
            Order order,
            FundingSourceService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expectedResult = order.FundingSourceOnlyGms;

            var result = await service.GetFundingSource(order.CallOffId);

            Assert.NotNull(result);
            result.Value.Should().Be(expectedResult!.Value);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetFundingSource_NullOrder_ThrowsException(
            bool? onlyGms,
            FundingSourceService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetFundingSource(null, onlyGms));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetFundingSource_NullOnlyGms_ThrowsException(
            Order order,
            FundingSourceService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetFundingSource(order, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task SetFundingSource_UpdatesFundingSource(
            Order order,
            bool? onlyGms,
            FundingSourceService service)
        {
            order.FundingSourceOnlyGms.Should().NotBeNull();

            await service.SetFundingSource(order, onlyGms);

            order.FundingSourceOnlyGms.Should().Be(onlyGms);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task SetFundingSource_SavesToDb(
            [Frozen] ApplicationDbContext context,
            Order order,
            bool? onlyGms,
            FundingSourceService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();
            order.FundingSourceOnlyGms.Should().NotBeNull();

            await service.SetFundingSource(order, onlyGms);

            var expectedOrder = context.Set<Order>().First(o => o.Equals(order));

            expectedOrder.FundingSourceOnlyGms.Should().Be(onlyGms);
        }
    }
}
