using System;
using System.Diagnostics.CodeAnalysis;
using EnumsNET;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemRequestTests
    {
        [Test]
        [OrderingAutoData]
        public static void Constructor_NullOrder_ThrowsException(
            CatalogueItemType catalogueItemType)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateOrderItemRequest(
                null,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                0));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_NullCatalogueItemType_ThrowsException(
            Order order)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateOrderItemRequest(
                order,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                0));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Order(
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                null);

            request.Order.Should().Be(order);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_OdsCode(
            string odsCode,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                odsCode,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                null);

            request.OdsCode.Should().Be(odsCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CatalogueItemId(
            string catalogueItemId,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                catalogueItemId,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                null);

            request.CatalogueItemId.Should().Be(catalogueItemId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CatalogueItemType(
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                null);

            request.CatalogueItemType.Should().Be(catalogueItemType);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CatalogueItemName(
            string catalogueItemName,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                catalogueItemName,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                null);

            request.CatalogueItemName.Should().Be(catalogueItemName);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CatalogueSolutionId(
            string catalogueSolutionId,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                catalogueSolutionId,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                null);

            request.CatalogueSolutionId.Should().Be(catalogueSolutionId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_ProvisioningType(
            ProvisioningType provisioningType,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                provisioningType.GetName(),
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                null);

            request.ProvisioningType.Should().Be(provisioningType);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CataloguePriceType(
            CataloguePriceType cataloguePriceType,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                cataloguePriceType.GetName(),
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                null);

            request.CataloguePriceType.Should().Be(cataloguePriceType);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CataloguePriceUnitTierName(
            string cataloguePriceUnitTierName,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                cataloguePriceUnitTierName,
                null,
                null,
                null,
                0,
                null,
                null,
                null);

            request.CataloguePriceUnitTierName.Should().Be(cataloguePriceUnitTierName);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CataloguePriceUnitDescription(
            string cataloguePriceUnitDescription,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                cataloguePriceUnitDescription,
                null,
                null,
                0,
                null,
                null,
                null);

            request.CataloguePriceUnitDescription.Should().Be(cataloguePriceUnitDescription);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CataloguePriceTimeUnit(
            TimeUnit timeUnit,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                timeUnit,
                null,
                0,
                null,
                null,
                null);

            request.PriceTimeUnit.Should().Be(timeUnit);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CurrencyCode(
            string currencyCode,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                currencyCode,
                0,
                null,
                null,
                null);

            request.CurrencyCode.Should().Be(currencyCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Quantity(
            int quantity,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                quantity,
                null,
                null,
                null);

            request.Quantity.Should().Be(quantity);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_EstimationPeriod(
            TimeUnit estimationPeriod,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                estimationPeriod.GetDisplayName(),
                null,
                null);

            request.EstimationPeriod.Should().Be(estimationPeriod);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_EstimationPeriod(
            DateTime deliveryDate,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                deliveryDate,
                null);

            request.DeliveryDate.Should().Be(deliveryDate);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Price(
            decimal price,
            Order order,
            CatalogueItemType catalogueItemType)
        {
            var request = new CreateOrderItemRequest(
                order,
                null,
                null,
                catalogueItemType,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                null,
                null,
                price);

            request.Price.Should().Be(price);
        }
    }
}
