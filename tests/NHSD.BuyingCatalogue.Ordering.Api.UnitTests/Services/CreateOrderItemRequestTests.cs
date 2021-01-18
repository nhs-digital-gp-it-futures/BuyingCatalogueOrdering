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

        [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by AutoFixture")]
        public sealed class TestOrderItemRequest : CreateOrderItemRequest
        {
            public TestOrderItemRequest(Order order, CreateOrderItemModel model, CatalogueItemType itemType)
                : base(order, model, itemType)
            {
            }
        }
    }
}
