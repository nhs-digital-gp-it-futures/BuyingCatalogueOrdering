using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemAssociatedServiceModelTests
    {
        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedOdsCode(
            Order order,
            CreateOrderItemAssociatedServiceModel model)
        {
            var request = model.ToRequest(order);

            request.OdsCode.Should().Be(order.OrganisationOdsCode);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCatalogueItemType(
            Order order,
            CreateOrderItemAssociatedServiceModel model)
        {
            var request = model.ToRequest(order);

            request.CatalogueItemType.Should().Be(CatalogueItemType.AssociatedService);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCatalogueItemId(
            Order order,
            CreateOrderItemAssociatedServiceModel model)
        {
            var request = model.ToRequest(order);

            request.CatalogueSolutionId.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedPriceTimeUnit(
            Order order,
            CreateOrderItemAssociatedServiceModel model)
        {
            var request = model.ToRequest(order);

            request.PriceTimeUnit.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedDeliveryDate(
            Order order,
            CreateOrderItemAssociatedServiceModel model)
        {
            var request = model.ToRequest(order);

            request.DeliveryDate.Should().BeNull();
        }
    }
}
