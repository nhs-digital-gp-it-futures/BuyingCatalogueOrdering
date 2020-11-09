using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
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
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = OrderingFixtureFactory.Create();
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CreateOrderItemAssociatedServiceRequest).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_NullModelQuantity_ThrowsException(
            Order order,
            CatalogueItemType itemType,
            CreateOrderItemModel model)
        {
            model.Quantity = null;

            Assert.Throws<ArgumentException>(() => _ = new TestOrderItemRequest(order, model, itemType));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_NullModelPrice_ThrowsException(
            Order order,
            CatalogueItemType itemType,
            CreateOrderItemModel model)
        {
            model.Price = null;

            Assert.Throws<ArgumentException>(() => _ = new TestOrderItemRequest(order, model, itemType));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Order(
            [Frozen] Order order,
            TestOrderItemRequest request)
        {
            request.Order.Should().Be(order);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CatalogueItemId(
            [Frozen] CreateOrderItemModel model,
            TestOrderItemRequest request)
        {
            request.CatalogueItemId.Should().Be(model.CatalogueItemId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CatalogueItemType(
            [Frozen] CatalogueItemType itemType,
            TestOrderItemRequest request)
        {
            request.CatalogueItemType.Should().Be(itemType);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CatalogueItemName(
            [Frozen] CreateOrderItemModel model,
            TestOrderItemRequest request)
        {
            request.CatalogueItemName.Should().Be(model.CatalogueItemName);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_ProvisioningType(
            [Frozen] ProvisioningType provisioningType,
            TestOrderItemRequest request)
        {
            request.ProvisioningType.Should().Be(provisioningType);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CataloguePriceType(
            [Frozen] CataloguePriceType priceType,
            TestOrderItemRequest request)
        {
            request.CataloguePriceType.Should().Be(priceType);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CataloguePriceUnitTierName(
            [Frozen] ItemUnitModel model,
            TestOrderItemRequest request)
        {
            request.CataloguePriceUnitTierName.Should().Be(model.Name);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CataloguePriceUnitDescription(
            [Frozen] ItemUnitModel model,
            TestOrderItemRequest request)
        {
            request.CataloguePriceUnitDescription.Should().Be(model.Description);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CurrencyCode(
            [Frozen] CreateOrderItemModel model,
            TestOrderItemRequest request)
        {
            request.CurrencyCode.Should().Be(model.CurrencyCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Quantity(
            [Frozen] CreateOrderItemModel model,
            TestOrderItemRequest request)
        {
            request.Quantity.Should().Be(model.Quantity);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_EstimationPeriod(
            [Frozen] TimeUnit timeUnit,
            TestOrderItemRequest request)
        {
            request.EstimationPeriod.Should().Be(timeUnit);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_Price(
            [Frozen] decimal price,
            TestOrderItemRequest request)
        {
            request.Price.Should().Be(price);
        }

        public sealed class TestOrderItemRequest : CreateOrderItemRequest
        {
            public TestOrderItemRequest(Order order, CreateOrderItemModel model, CatalogueItemType itemType)
                : base(order, model, itemType)
            {
            }
        }
    }
}
