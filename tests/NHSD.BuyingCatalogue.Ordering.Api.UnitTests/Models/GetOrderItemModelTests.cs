using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models
{
    [TestFixture]
    internal sealed class GetOrderItemModelTests
    {
        [Test]
        public void Constructor_NullOrderItem_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new GetOrderItemModel(string.Empty, null, ServiceRecipientBuilder.Create().Build()));
        }

        [Test]
        public void Constructor_ReturnsExpected()
        {
            const string orderId = "Some order id";

            var serviceRecipient = ServiceRecipientBuilder
                .Create()
                .WithOdsCode("ODS1")
                .Build();

            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(123)
                .WithOdsCode(serviceRecipient.OdsCode)
                .Build();

            var actual = new GetOrderItemModel(orderId, orderItem, serviceRecipient);

            var expected = new
            {
                orderItem.OrderItemId,
                orderItem.CatalogueItemId,
                CatalogueItemType = orderItem.CatalogueItemType.Name,
                orderItem.CatalogueItemName,
                orderItem.CurrencyCode,
                orderItem.DeliveryDate,
                ItemUnit = new
                {
                    orderItem.CataloguePriceUnit.Name,
                    orderItem.CataloguePriceUnit.Description,
                },
                orderItem.Price,
                ProvisioningType = orderItem.ProvisioningType.Name,
                orderItem.Quantity,
                ServiceRecipient = new
                {
                    orderItem.OdsCode,
                    serviceRecipient.Name
                },
                TimeUnit = orderItem.PriceTimeUnit is null ? null : new
                {
                    orderItem.PriceTimeUnit?.Name, 
                    orderItem.PriceTimeUnit?.Description
                },
                Type = orderItem.CataloguePriceType.Name
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Constructor_NullServiceRecipient_ServiceRecipientIsNull()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(456)
                .WithOdsCode("Some ods code")
                .Build();

            var actual = new GetOrderItemModel("Some order id", orderItem, null);

            actual.ServiceRecipient.Should().BeNull();
        }
    }
}
