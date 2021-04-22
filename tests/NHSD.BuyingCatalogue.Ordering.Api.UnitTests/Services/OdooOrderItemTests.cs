using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Application;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OdooOrderItemTests
    {
        [Test]
        public static void Constructors_NullOrderItem_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OdooOrderItem(null));
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesCallOffAgreementId([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CallOffAgreementId.Should().Be(orderItem.Order.CallOffId.ToString());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesCallOffOrderingPartyId([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CallOffOrderingPartyId.Should().Be(orderItem.Order.OrderingParty.OdsCode);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesCallOffOrderingPartyName([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CallOffOrderingPartyName.Should().Be(orderItem.Order.OrderingParty.Name);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesCallOffCommencementDate([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CallOffCommencementDate.Should().Be(orderItem.Order.CommencementDate);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesServiceRecipientId([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ServiceRecipientId.Should().Be(orderItem.Recipient.OdsCode);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesServiceRecipientName(
            [Frozen] FlattenedOrderItem orderItem,
            OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ServiceRecipientName.Should().Be(orderItem.Recipient.Name);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesServiceRecipientItemId(
            [Frozen] FlattenedOrderItem orderItem,
            OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ServiceRecipientItemId.Should().Be($"{orderItem.Order.CallOffId}-{orderItem.Recipient.OdsCode}-{orderItem.ItemId}");
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesSupplierId([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.SupplierId.Should().Be(orderItem.Order.Supplier.Id);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesSupplierName([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.SupplierName.Should().Be(orderItem.Order.Supplier.Name);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesProductId([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ProductId.Should().Be(orderItem.CatalogueItem.Id.ToString());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesProductName([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ProductName.Should().Be(orderItem.CatalogueItem.Name);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesProductType([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ProductType.Should().Be(orderItem.CatalogueItem.CatalogueItemType.DisplayName());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesQuantityOrdered([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.QuantityOrdered.Should().Be(orderItem.Quantity);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesUnitOfOrder([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.UnitOfOrder.Should().Be(orderItem.PricingUnit.Description);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_NullUnitTime_InitializesUnitTime(
            Order order,
            CatalogueItem catalogueItem,
            PricingUnit pricingUnit,
            ServiceRecipient recipient)
        {
            var orderItem = new FlattenedOrderItem
            {
                CatalogueItem = catalogueItem,
                Order = order,
                PricingUnit = pricingUnit,
                Recipient = recipient,
            };

            var odooOrderItem = new OdooOrderItem(orderItem);

            odooOrderItem.UnitTime.Should().BeNull();
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_OnDemandProvisioningType_InitializesEstimationPeriod(
            Order order,
            CatalogueItem catalogueItem,
            PricingUnit pricingUnit,
            ServiceRecipient recipient)
        {
            var orderItem = new FlattenedOrderItem
            {
                CatalogueItem = catalogueItem,
                Order = order,
                PricingUnit = pricingUnit,
                ProvisioningType = ProvisioningType.OnDemand,
                Recipient = recipient,
            };

            var odooOrderItem = new OdooOrderItem(orderItem);

            odooOrderItem.UnitTime.Should().BeNull();
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesUnitTime(
            [Frozen] TimeUnit priceTimeUnit,
            OdooOrderItem odooOrderItem)
        {
            odooOrderItem.UnitTime.Should().Be(priceTimeUnit.Description());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_DeclarativeProvisioningType_InitializesEstimationPeriod(
            Order order,
            CatalogueItem catalogueItem,
            PricingUnit pricingUnit,
            ServiceRecipient recipient)
        {
            var orderItem = new FlattenedOrderItem
            {
                CatalogueItem = catalogueItem,
                Order = order,
                PricingUnit = pricingUnit,
                ProvisioningType = ProvisioningType.Declarative,
                Recipient = recipient,
            };

            var odooOrderItem = new OdooOrderItem(orderItem);

            odooOrderItem.EstimationPeriod.Should().BeNull();
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_OnDemandProvisioningType_InitializesEstimationPeriod(
            [Frozen] TimeUnit estimationPeriod,
            OdooOrderItem odooOrderItem)
        {
            odooOrderItem.EstimationPeriod.Should().Be(estimationPeriod.Description());
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_NullEstimationPeriod_InitializesEstimationPeriod(FlattenedOrderItem orderItem)
        {
            orderItem.EstimationPeriod = null;

            var odooOrderItem = new OdooOrderItem(orderItem);

            odooOrderItem.EstimationPeriod.Should().BeNull();
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesPrice([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.Price.Should().Be(orderItem.Price);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_NullPrice_InitializesPrice(FlattenedOrderItem orderItem)
        {
            orderItem.Price = null;

            var odooOrderItem = new OdooOrderItem(orderItem);

            odooOrderItem.Price.Should().Be(0.00m);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesOrderType([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.OrderType.Should().Be((int)orderItem.ProvisioningType);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesFundingType(OdooOrderItem odooOrderItem)
        {
            odooOrderItem.FundingType.Should().Be("Central");
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesM1Planned([Frozen] FlattenedOrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.M1Planned.Should().Be(orderItem.DeliveryDate);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesActualM1Date(OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ActualM1Date.Should().BeNull();
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesBuyerVerificationDate(OdooOrderItem odooOrderItem)
        {
            odooOrderItem.BuyerVerificationDate.Should().BeNull();
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesCeaseDate(OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CeaseDate.Should().BeNull();
        }
    }
}
