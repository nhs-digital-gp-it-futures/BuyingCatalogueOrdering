using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
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
        [OrderingAutoData]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_NullOrder_ThrowsArgumentNullException(OrderItem orderItem)
        {
            Assert.Throws<ArgumentNullException>(() => new OdooOrderItem(null, orderItem, null));
        }

        [Test]
        [OrderingAutoData]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_NullOrderItem_ThrowsArgumentNullException(Order order)
        {
            Assert.Throws<ArgumentNullException>(() => new OdooOrderItem(order, null, null));
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesCallOffAgreementId([Frozen] Order order, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CallOffAgreementId.Should().Be(order.OrderId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesCallOffOrderingPartyId([Frozen] Order order, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CallOffOrderingPartyId.Should().Be(order.OrganisationOdsCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesCallOffOrderingPartyName([Frozen] Order order, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CallOffOrderingPartyName.Should().Be(order.OrganisationName);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesCallOffCommencementDate([Frozen] Order order, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CallOffCommencementDate.Should().Be(order.CommencementDate);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesServiceRecipientId([Frozen] OrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ServiceRecipientId.Should().Be(orderItem.OdsCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesServiceRecipientName(
            Order order,
            OrderItem orderItem,
            string serviceRecipientName)
        {
            var odooOrderItem = new OdooOrderItem(order, orderItem, serviceRecipientName);

            odooOrderItem.ServiceRecipientName.Should().Be(serviceRecipientName);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesServiceRecipientItemId(
            [Frozen] Order order,
            [Frozen] OrderItem orderItem,
            OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ServiceRecipientItemId.Should().Be($"{order.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}");
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesSupplierId([Frozen] Order order, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.SupplierId.Should().Be(order.SupplierId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesSupplierName([Frozen] Order order, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.SupplierName.Should().Be(order.SupplierName);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesProductId([Frozen] OrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ProductId.Should().Be(orderItem.CatalogueItemId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesProductName([Frozen] OrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ProductName.Should().Be(orderItem.CatalogueItemName);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesProductType([Frozen] OrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ProductType.Should().Be(orderItem.CatalogueItemType.DisplayName());
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesQuantityOrdered([Frozen] OrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.QuantityOrdered.Should().Be(orderItem.Quantity);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesUnitOfOrder([Frozen] OrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.UnitOfOrder.Should().Be(orderItem.CataloguePriceUnit.Description);
        }

        [Test]
        public static void Constructor_NullUnitTime_InitializesUnitTime()
        {
            var fixture = new Fixture();
            fixture.Customize(new OrderingCustomization());
            fixture.Register<TimeUnit?>(() => null);

            var odooOrderItem = fixture.Create<OdooOrderItem>();

            odooOrderItem.UnitTime.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesUnitTime(
            [Frozen] TimeUnit priceTimeUnit,
            OdooOrderItem odooOrderItem)
        {
            odooOrderItem.UnitTime.Should().Be(priceTimeUnit.Description());
        }

        [Test]
        public static void Constructor_DeclarativeProvisioningType_InitializesEstimationPeriod()
        {
            var fixture = new Fixture();
            fixture.Customize(new OrderingCustomization());
            fixture.Register(() => ProvisioningType.Declarative);

            var odooOrderItem = fixture.Create<OdooOrderItem>();

            odooOrderItem.EstimationPeriod.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_OnDemandProvisioningType_InitializesEstimationPeriod(
            [Frozen] TimeUnit estimationPeriod,
            OdooOrderItem odooOrderItem)
        {
            odooOrderItem.EstimationPeriod.Should().Be(estimationPeriod.Description());
        }

        [Test]
        public static void Constructor_NullEstimationPeriod_InitializesEstimationPeriod()
        {
            var fixture = new Fixture();
            fixture.Customize(new OrderingCustomization());
            fixture.Register(() => ProvisioningType.Declarative);

            var odooOrderItem = fixture.Create<OdooOrderItem>();

            odooOrderItem.EstimationPeriod.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesPrice([Frozen] OrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.Price.Should().Be(orderItem.Price);
        }

        [Test]
        public static void Constructor_NullPrice_InitializesPrice()
        {
            var fixture = new Fixture();
            fixture.Customize(new OrderingCustomization());
            fixture.Register<decimal?>(() => null);

            var odooOrderItem = fixture.Create<OdooOrderItem>();

            odooOrderItem.Price.Should().Be(0.00m);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesOrderType([Frozen] OrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.OrderType.Should().Be((int)orderItem.ProvisioningType);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesFundingType(OdooOrderItem odooOrderItem)
        {
            odooOrderItem.FundingType.Should().Be("Central");
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesM1Planned([Frozen] OrderItem orderItem, OdooOrderItem odooOrderItem)
        {
            odooOrderItem.M1Planned.Should().Be(orderItem.DeliveryDate);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesActualM1Date(OdooOrderItem odooOrderItem)
        {
            odooOrderItem.ActualM1Date.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesBuyerVerificationDate(OdooOrderItem odooOrderItem)
        {
            odooOrderItem.BuyerVerificationDate.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesCeaseDate(OdooOrderItem odooOrderItem)
        {
            odooOrderItem.CeaseDate.Should().BeNull();
        }
    }
}
