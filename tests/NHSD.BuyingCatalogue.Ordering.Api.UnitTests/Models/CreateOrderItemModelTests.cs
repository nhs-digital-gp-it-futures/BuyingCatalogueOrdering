using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
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
    internal static class CreateOrderItemModelTests
    {
        [Test]
        public static void ToRequest_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new OrderCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var method = typeof(CreateOrderItemModel).GetMethod(nameof(CreateOrderItemModel.ToRequest));

            assertion.Verify(method);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_NullQuantity_ThrowsInvalidOperationException(
            Order order,
            CreateOrderItemModel model)
        {
            model.Quantity = null;

            Assert.Throws<InvalidOperationException>(() => model.ToRequest(order));
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCatalogueItemId(
            Order order,
            CreateOrderItemModel model)
        {
            var request = model.ToRequest(order);

            request.CatalogueItemId.Should().Be(model.CatalogueItemId);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCatalogueItemName(
            Order order,
            CreateOrderItemModel model)
        {
            var request = model.ToRequest(order);

            request.CatalogueItemName.Should().Be(model.CatalogueItemName);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedProvisioningType(
            ProvisioningType provisioningType,
            Order order,
            CreateOrderItemModel model)
        {
            model.ProvisioningType = provisioningType.ToString();

            var request = model.ToRequest(order);

            request.ProvisioningType.Should().Be(provisioningType);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCataloguePriceType(
            CataloguePriceType cataloguePriceType,
            Order order,
            CreateOrderItemModel model)
        {
            model.Type = cataloguePriceType.ToString();

            var request = model.ToRequest(order);

            request.CataloguePriceType.Should().Be(cataloguePriceType);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCataloguePriceUnitDescription(
            [Frozen] ItemUnitModel itemUnit,
            Order order,
            CreateOrderItemModel model)
        {
            var request = model.ToRequest(order);

            request.CataloguePriceUnitDescription.Should().Be(itemUnit.Description);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCataloguePriceUnitTierName(
            [Frozen] ItemUnitModel itemUnit,
            Order order,
            CreateOrderItemModel model)
        {
            var request = model.ToRequest(order);

            request.CataloguePriceUnitTierName.Should().Be(itemUnit.Name);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCurrencyCode(
            Order order,
            CreateOrderItemModel model)
        {
            var request = model.ToRequest(order);

            request.CurrencyCode.Should().Be(model.CurrencyCode);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedQuantity(
            Order order,
            CreateOrderItemModel model)
        {
            var request = model.ToRequest(order);

            request.Quantity.Should().Be(model.Quantity);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedEstimationPeriod(
            TimeUnit estimationPeriod,
            Order order,
            CreateOrderItemModel model)
        {
            model.EstimationPeriod = estimationPeriod.Name();

            var request = model.ToRequest(order);

            request.EstimationPeriod.Should().Be(estimationPeriod);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedOrder(
            Order order,
            CreateOrderItemModel model)
        {
            var request = model.ToRequest(order);

            request.Order.Should().Be(order);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedPrice(
            Order order,
            CreateOrderItemModel model)
        {
            var request = model.ToRequest(order);

            request.Price.Should().Be(model.Price);
        }
    }
}
