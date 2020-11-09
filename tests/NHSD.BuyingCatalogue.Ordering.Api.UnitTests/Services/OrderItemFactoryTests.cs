using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderItemFactoryTests
    {
        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedCatalogueItemType(
            [Frozen] CatalogueItemType itemType,
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.CatalogueItemType.Should().Be(itemType);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedProvisioningType(
            [Frozen] ProvisioningType provisioningType,
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.ProvisioningType.Should().Be(provisioningType);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedOdsCode(
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.OdsCode.Should().Be(request.OdsCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedCatalogueItemId(
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.CatalogueItemId.Should().Be(request.CatalogueItemId);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedCatalogueItemName(
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.CatalogueItemName.Should().Be(request.CatalogueItemName);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedParentCatalogueItemId(
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.ParentCatalogueItemId.Should().Be(request.CatalogueSolutionId);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedParentCataloguePriceType(
            [Frozen] CataloguePriceType priceType,
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.CataloguePriceType.Should().Be(priceType);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedCurrencyCode(
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.CurrencyCode.Should().Be(request.CurrencyCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedQuantity(
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.Quantity.Should().Be(request.Quantity);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedDeliveryDate(
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.DeliveryDate.Should().Be(request.DeliveryDate);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedPrice(
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.Price.Should().Be(request.Price);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedCataloguePriceUnit(
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.CataloguePriceUnit.Should().NotBeNull();
            orderItem.CataloguePriceUnit.Name.Should().Be(request.CataloguePriceUnitTierName);
            orderItem.CataloguePriceUnit.Description.Should().Be(request.CataloguePriceUnitDescription);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedPriceTimeUnit(
            [Frozen] TimeUnit priceTimeUnit,
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.PriceTimeUnit.Should().Be(priceTimeUnit);
        }

        [Test]
        [OrderingAutoData]
        public static void Create_HasExpectedEstimationPeriod(
            [Frozen] TimeUnit estimationPeriod,
            CreateOrderItemSolutionRequest request,
            OrderItemFactory factory)
        {
            var orderItem = factory.Create(request);

            orderItem.EstimationPeriod.Should().Be(estimationPeriod);
        }
    }
}
