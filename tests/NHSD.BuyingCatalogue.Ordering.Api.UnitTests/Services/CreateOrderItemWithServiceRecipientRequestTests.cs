using System.Diagnostics.CodeAnalysis;
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
    internal static class CreateOrderItemWithServiceRecipientRequestTests
    {
        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_OdsCode(
            [Frozen] ServiceRecipientModel model,
            TestOrderItemRequest request)
        {
            request.OdsCode.Should().Be(model.OdsCode);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_NullServiceRecipient_OdsCodeIsNull(
            Order order,
            CatalogueItemType itemType,
            CreateOrderItemModel model)
        {
            model.ServiceRecipient = null;

            var request = new TestOrderItemRequest(order, model, itemType);

            request.OdsCode.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CataloguePriceTimeUnit(
            [Frozen] TimeUnit timeUnit,
            TestOrderItemRequest request)
        {
            request.PriceTimeUnit.Should().Be(timeUnit);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_NullTimeUnit_Initializes_CataloguePriceTimeUnit(
            Order order,
            CatalogueItemType itemType,
            CreateOrderItemModel model)
        {
            model.TimeUnit = null;

            var request = new TestOrderItemRequest(order, model, itemType);

            request.PriceTimeUnit.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_ServiceRecipient(
            Order order,
            CatalogueItemType itemType,
            [Frozen] ServiceRecipientModel expectedRecipient,
            CreateOrderItemModel model)
        {
            var request = new TestOrderItemRequest(order, model, itemType);

            var actualServiceRecipient = request.ServiceRecipient;
            actualServiceRecipient.Should().NotBeNull();
            actualServiceRecipient.Name.Should().Be(expectedRecipient.Name);
            actualServiceRecipient.OdsCode.Should().Be(expectedRecipient.OdsCode);
            actualServiceRecipient.OrderId.Should().Be(order.OrderId);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_NullServiceRecipient_ServiceRecipientIsNull(
            Order order,
            CatalogueItemType itemType,
            CreateOrderItemModel model)
        {
            model.ServiceRecipient = null;

            var request = new TestOrderItemRequest(order, model, itemType);

            request.ServiceRecipient.Should().BeNull();
        }

        public sealed class TestOrderItemRequest : CreateOrderItemWithServiceRecipientRequest
        {
            public TestOrderItemRequest(Order order, CreateOrderItemModel model, CatalogueItemType itemType)
                : base(order, model, itemType)
            {
            }
        }
    }
}
