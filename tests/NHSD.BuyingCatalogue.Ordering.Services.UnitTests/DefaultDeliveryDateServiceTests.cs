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
    internal static class DefaultDeliveryDateServiceTests
    {
        [Test]
        public static void Constructor_NullHttpContextAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new DefaultDeliveryDateService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_DefaultDeliveryDateDoesNotExist(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            DefaultDeliveryDateService service)
        {
            var result = await service.GetDefaultDeliveryDate(callOffId, catalogueItemId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetDefaultDeliveryDate_ReturnsDefaultDeliveryDate(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            DateTime defaultDeliveryDate,
            Order order,
            DefaultDeliveryDateService service)
        {
            order.SetDefaultDeliveryDate(catalogueItemId, defaultDeliveryDate);
            context.Add(order);
            await context.SaveChangesAsync();

            var response = await service.GetDefaultDeliveryDate(callOffId, catalogueItemId);
            var actualDate = response.Value;

            actualDate.Should().Be(defaultDeliveryDate);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_AddDefaultDeliveryDate(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            DateTime defaultDeliveryDate,
            Order order,
            DefaultDeliveryDateService service)
        {
            context.Add(order);
            await context.SaveChangesAsync();

            DeliveryDateResult result = await service.SetDefaultDeliveryDate(callOffId, catalogueItemId, defaultDeliveryDate);

            result.Should().Be(DeliveryDateResult.Added);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesDefaultDeliveryDate(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            DateTime defaultDeliveryDate,
            Order order,
            DefaultDeliveryDateService service)
        {
            order.SetDefaultDeliveryDate(catalogueItemId, defaultDeliveryDate);
            context.Add(order);
            await context.SaveChangesAsync();

            DeliveryDateResult result = await service.SetDefaultDeliveryDate(callOffId, catalogueItemId, defaultDeliveryDate.AddDays(-1));

            result.Should().Be(DeliveryDateResult.Updated);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderDoesNotExist(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            DefaultDeliveryDateService service)
        {
            var result = await service.GetDefaultDeliveryOrder(callOffId, catalogueItemId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrder_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            DateTime defaultDeliveryDate,
            Order order,
            DefaultDeliveryDateService service)
        {
            order.SetDefaultDeliveryDate(catalogueItemId, defaultDeliveryDate);
            context.Add(order);
            await context.SaveChangesAsync();

            var actual = await service.GetDefaultDeliveryOrder(callOffId, catalogueItemId);

            actual.CallOffId.Should().Be(order.CallOffId);
            actual.DefaultDeliveryDates.Should().BeEquivalentTo(order.DefaultDeliveryDates);
        }
    }
}
