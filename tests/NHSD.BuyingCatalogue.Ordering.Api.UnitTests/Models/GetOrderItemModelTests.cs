using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class GetOrderItemModelTests
    {
        [Test]
        [CommonAutoData]
        public static void Constructor_NullOrderItem_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new GetOrderItemModel(null));
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesServiceRecipients(
            [Frozen] IEnumerable<OrderItemRecipient> recipients,
            GetOrderItemModel model)
        {
            model.ServiceRecipients.Should().BeEquivalentTo(recipients.ToList().ToModelList());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesCatalogueItemType(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.CatalogueItemType.Should().Be(orderItem.CatalogueItem.CatalogueItemType.ToString());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesCatalogueItemName(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.CatalogueItemName.Should().Be(orderItem.CatalogueItem.Name);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesCatalogueItemId(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.CatalogueItemId.Should().Be(orderItem.CatalogueItem.Id.ToString());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesCurrencyCode(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.CurrencyCode.Should().Be(orderItem.CurrencyCode);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesItemUnit(
            [Frozen] PricingUnit priceUnit,
            GetOrderItemModel model)
        {
            model.ItemUnit.Should().NotBeNull();
            model.ItemUnit.Description.Should().Be(priceUnit.Description);
            model.ItemUnit.Name.Should().Be(priceUnit.Name);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesPrice(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.Price.Should().Be(orderItem.Price);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesProvisioningType(
            [Frozen] ProvisioningType provisioningType,
            GetOrderItemModel model)
        {
            model.ProvisioningType.Should().Be(provisioningType.ToString());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesTimeUnit(
            [Frozen] TimeUnit timeUnit,
            GetOrderItemModel model)
        {
            model.TimeUnit.Should().NotBeNull();
            model.TimeUnit.Description.Should().Be(timeUnit.Description());
            model.TimeUnit.Name.Should().Be(timeUnit.Name());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesType(
            [Frozen] CataloguePriceType priceType,
            GetOrderItemModel model)
        {
            model.Type.Should().Be(priceType.ToString());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesEstimationPeriod(
            [Frozen] TimeUnit estimationPeriod,
            GetOrderItemModel model)
        {
            model.EstimationPeriod.Should().Be(estimationPeriod.Name());
        }
    }
}
