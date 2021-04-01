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
    internal static class FundingSourceServiceTests
    {
        [Test]
        public static void Constructor_NullHttpContextAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new FundingSourceService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_FundingSourceDoesNotExist(
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
            Order order1,
            Order order2,
            FundingSourceService service)
        {
            context.Order.AddRange(order1, order2);
            await context.SaveChangesAsync();

            var expectedResult = order2.FundingSourceOnlyGms;

            var result = await service.GetFundingSource(order2.CallOffId);

            Assert.NotNull(result);
            result.Value.Should().Be(expectedResult!.Value);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_NullOrder_ThrowsException(
            bool? onlyGms,
            FundingSourceService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetFundingSource(null, onlyGms));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_NullOnlyGms_ThrowsException(
            Order order,
            FundingSourceService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetFundingSource(order, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesFundingSource(
            Order order,
            bool? onlyGms,
            FundingSourceService service)
        {
            order.FundingSourceOnlyGms.Should().NotBeNull();

            await service.SetFundingSource(order, onlyGms);

            order.FundingSourceOnlyGms.Should().Be(onlyGms);
        }
    }
}
