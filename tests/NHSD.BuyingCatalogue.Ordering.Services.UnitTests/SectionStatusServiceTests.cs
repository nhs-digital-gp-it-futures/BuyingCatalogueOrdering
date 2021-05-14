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
    internal static class SectionStatusServiceTests
    {
        [Test]
        public static void Constructor_NullAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new SectionStatusService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrder_ReturnsNull(
            CallOffId callOffId,
            SectionStatusService service)
        {
            var result = await service.GetOrder(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrder_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            SectionStatusService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var result = await service.GetOrder(order.CallOffId);

            result.Should().BeEquivalentTo(order);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetSectionStatus_NullOrder_ThrowsException(
            string sectionId,
            SectionStatusService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetSectionStatus(null, sectionId));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetSectionStatus_NullSectionId_ThrowsException(
            Order order,
            SectionStatusService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetSectionStatus(order, null));
        }

        [Test]
        [InMemoryDbInlineAutoData("additional-services", true, false, false)]
        [InMemoryDbInlineAutoData("catalogue-solutions", false, true, false)]
        [InMemoryDbInlineAutoData("associated-services", false, false, true)]
        public static async Task SetSectionStatus_WithSectionId_UpdatesTheSelectedStatus(
            string sectionId,
            bool additionalServicesViewed,
            bool catalogueSolutionsViewed,
            bool associatedServicesViewed,
            Order order,
            SectionStatusService service)
        {
            order.Progress.AdditionalServicesViewed = false;
            order.Progress.AssociatedServicesViewed = false;
            order.Progress.CatalogueSolutionsViewed = false;

            await service.SetSectionStatus(order, sectionId);

            order.Progress.AdditionalServicesViewed.Should().Be(additionalServicesViewed);
            order.Progress.AssociatedServicesViewed.Should().Be(associatedServicesViewed);
            order.Progress.CatalogueSolutionsViewed.Should().Be(catalogueSolutionsViewed);
        }

        [Test]
        [InMemoryDbInlineAutoData("additional-services", true, false, false)]
        [InMemoryDbInlineAutoData("catalogue-solutions", false, true, false)]
        [InMemoryDbInlineAutoData("associated-services", false, false, true)]
        public static async Task SetSectionStatus_WithSectionId_SavesToDb(
            string sectionId,
            bool additionalServicesViewed,
            bool catalogueSolutionsViewed,
            bool associatedServicesViewed,
            [Frozen] ApplicationDbContext context,
            Order order,
            SectionStatusService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            order.Progress.AdditionalServicesViewed = false;
            order.Progress.AssociatedServicesViewed = false;
            order.Progress.CatalogueSolutionsViewed = false;

            await service.SetSectionStatus(order, sectionId);

            var expectedOrder = context.Set<Order>().First(o => o.Equals(order));

            expectedOrder.Progress.AdditionalServicesViewed.Should().Be(additionalServicesViewed);
            expectedOrder.Progress.AssociatedServicesViewed.Should().Be(associatedServicesViewed);
            expectedOrder.Progress.CatalogueSolutionsViewed.Should().Be(catalogueSolutionsViewed);
        }
    }
}
