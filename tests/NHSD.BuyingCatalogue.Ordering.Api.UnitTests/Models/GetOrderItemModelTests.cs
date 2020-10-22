using System;
using System.Diagnostics.CodeAnalysis;
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
    internal static class GetOrderItemModelTests
    {
        [Test]
        [OrderingAutoData]
        public static void Constructor_NullOrderItem_ThrowsArgumentNullException(
            ServiceRecipient serviceRecipient)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new GetOrderItemModel(null, serviceRecipient));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesOrderItemId(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.OrderItemId.Should().Be(orderItem.OrderItemId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesServiceRecipient(
            [Frozen] ServiceRecipient serviceRecipient,
            GetOrderItemModel model)
        {
            model.ServiceRecipient.Should().NotBeNull();
            model.ServiceRecipient.Name.Should().Be(serviceRecipient.Name);
            model.ServiceRecipient.OdsCode.Should().Be(serviceRecipient.OdsCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesCatalogueItemType(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.CatalogueItemType.Should().Be(orderItem.CatalogueItemType.Name);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesCatalogueItemName(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.CatalogueItemName.Should().Be(orderItem.CatalogueItemName);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesCatalogueItemId(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.CatalogueItemId.Should().Be(orderItem.CatalogueItemId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesCurrencyCode(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.CurrencyCode.Should().Be(orderItem.CurrencyCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesDeliveryDate(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.DeliveryDate.Should().Be(orderItem.DeliveryDate);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesItemUnit(
            [Frozen] CataloguePriceUnit priceUnit,
            GetOrderItemModel model)
        {
            model.ItemUnit.Should().NotBeNull();
            model.ItemUnit.Description.Should().Be(priceUnit.Description);
            model.ItemUnit.Name.Should().Be(priceUnit.Name);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesPrice(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.Price.Should().Be(orderItem.Price);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesProvisioningType(
            [Frozen] ProvisioningType provisioningType,
            GetOrderItemModel model)
        {
            model.ProvisioningType.Should().Be(provisioningType.ToString());
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesQuantity(
            [Frozen] OrderItem orderItem,
            GetOrderItemModel model)
        {
            model.Quantity.Should().Be(orderItem.Quantity);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesTimeUnit(
            [Frozen] TimeUnit timeUnit,
            GetOrderItemModel model)
        {
            model.TimeUnit.Should().NotBeNull();
            model.TimeUnit.Description.Should().Be(timeUnit.Description());
            model.TimeUnit.Name.Should().Be(timeUnit.Name());
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesType(
            [Frozen] CataloguePriceType priceType,
            GetOrderItemModel model)
        {
            model.Type.Should().Be(priceType.ToString());
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesEstimationPeriod(
            [Frozen] TimeUnit estimationPeriod,
            GetOrderItemModel model)
        {
            model.EstimationPeriod.Should().Be(estimationPeriod.Name());
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_NullServiceRecipient_ServiceRecipientIsNull(
            OrderItem orderItem)
        {
            var model = new GetOrderItemModel(orderItem, null);

            model.ServiceRecipient.Should().BeNull();
        }
    }
}
