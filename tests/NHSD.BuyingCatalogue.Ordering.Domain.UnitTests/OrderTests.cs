using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrderTests
    {
        [Test]
        public void AddOrderItem_NullOrderItem_ThrowsArgumentNullException()
        {
            static void Test()
            {
                OrderBuilder
                    .Create()
                    .Build()
                    .AddOrderItem(null, Guid.Empty, string.Empty);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void AddOrderItem_OrderItem_ItemAdded()
        {
            var order = OrderBuilder.Create().Build();
            var orderItem = OrderItemBuilder.Create().Build();

            order.AddOrderItem(orderItem, Guid.Empty, String.Empty);

            var expected = new List<OrderItem> { orderItem };
            order.OrderItems.Should().BeEquivalentTo(expected);
        }

        [TestCase("Solution", true)]
        [TestCase("AdditionalService", false)]
        [TestCase("AssociatedService", false)]
        public void AddOrderItem_OrderItem_CatalogueItemType_CatalogueSolutionsViewedMatchExpectedValue(
            string catalogueItemTypeNameInput,
            bool expectedInput)
        {
            var order = OrderBuilder
                .Create()
                .WithCatalogueSolutionsViewed(false)
                .Build();

            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeNameInput))
                .Build();

            order.AddOrderItem(orderItem, Guid.Empty, String.Empty);

            order.CatalogueSolutionsViewed.Should().Be(expectedInput);
        }

        [TestCase("Solution", false)]
        [TestCase("AdditionalService", true)]
        [TestCase("AssociatedService", false)]
        public void AddOrderItem_OrderItem_CatalogueItemType_AdditionalServicesViewedMatchExpectedValue(
            string catalogueItemTypeNameInput,
            bool expectedInput)
        {
            var order = OrderBuilder
                .Create()
                .WithAdditionalServicesViewed(false)
                .Build();

            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeNameInput))
                .Build();

            order.AddOrderItem(orderItem, Guid.Empty, String.Empty);

            order.AdditionalServicesViewed.Should().Be(expectedInput);
        }

        [Test]
        public void AddOrderItem_AddSameOrderItem_ReturnsOneOrderItem()
        {
            var order = OrderBuilder.Create().Build();
            var orderItem = OrderItemBuilder.Create().Build();

            order.AddOrderItem(orderItem, Guid.Empty, String.Empty);
            order.AddOrderItem(orderItem, Guid.Empty, String.Empty);

            var expected = new List<OrderItem> { orderItem };
            order.OrderItems.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void AddOrderItem_AddDifferentOrderItem_ReturnsTwoOrderItems()
        {
            var order = OrderBuilder.Create().Build();
            var orderItem = OrderItemBuilder.Create().Build();
            var orderItemSecond = OrderItemBuilder.Create().Build();

            order.AddOrderItem(orderItem, Guid.Empty, String.Empty);
            order.AddOrderItem(orderItemSecond, Guid.Empty, String.Empty);

            var expected = new List<OrderItem> { orderItem, orderItemSecond };
            order.OrderItems.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void AddOrderItem_OrderItem_LastUpdatedByChanged()
        {
            var lastUpdatedBy = Guid.NewGuid();

            var order = OrderBuilder
                .Create()
                .Build();

            var orderItem = OrderItemBuilder.Create().Build();

            order.LastUpdatedBy.Should().NotBe(lastUpdatedBy);

            order.AddOrderItem(orderItem, lastUpdatedBy, String.Empty);

            order.LastUpdatedBy.Should().Be(lastUpdatedBy);
        }

        [Test]
        public void AddOrderItem_OrderItemAlreadyExists_LastUpdatedByNotChanged()
        {
            var lastUpdatedBy = Guid.NewGuid();

            var orderItem = OrderItemBuilder.Create().Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .WithLastUpdatedBy(lastUpdatedBy)
                .Build();

            order.AddOrderItem(orderItem, Guid.Empty, string.Empty);

            order.LastUpdatedBy.Should().Be(lastUpdatedBy);
        }

        [Test]
        public void AddOrderItem_OrderItem_LastUpdatedByNameChanged()
        {
            var lastUpdatedByName = Guid.NewGuid().ToString();

            var order = OrderBuilder
                .Create()
                .Build();

            var orderItem = OrderItemBuilder.Create().Build();

            order.LastUpdatedByName.Should().NotBe(lastUpdatedByName);

            order.AddOrderItem(orderItem, Guid.Empty, lastUpdatedByName);

            order.LastUpdatedByName.Should().Be(lastUpdatedByName);
        }

        [Test]
        public void AddOrderItem_OrderItemAlreadyExists_LastUpdatedByNameNotChanged()
        {
            var lastUpdatedByName = Guid.NewGuid().ToString();

            var orderItem = OrderItemBuilder.Create().Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .WithLastUpdatedByName(lastUpdatedByName)
                .Build();

            order.AddOrderItem(orderItem, Guid.Empty, "Should not be set");

            order.LastUpdatedByName.Should().Be(lastUpdatedByName);
        }

        [Test]
        public void UpdateOrderItem_OrderItemNotFound_NoOrderItemChange()
        {
            const int orderItemId = 1;
            const int unknownOrderItemId = 123;

            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(orderItemId)
                .WithPriceTimeUnit(TimeUnit.PerYear)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .Build();

            order.UpdateOrderItem(
                unknownOrderItemId,
                DateTime.UtcNow.AddDays(1),
                orderItem.Quantity + 1,
                TimeUnit.PerMonth,
                orderItem.Price + 1m,
                Guid.Empty,
                string.Empty);

            var expected = new
            {
                orderItem.DeliveryDate,
                orderItem.Quantity,
                orderItem.PriceTimeUnit,
                orderItem.Price
            };

            orderItem.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void UpdateOrderItem_OrderItemNotFound_NoOrderChange()
        {
            const int orderItemId = 1;
            const int unknownOrderItemId = 123;

            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(orderItemId)
                .WithPriceTimeUnit(TimeUnit.PerYear)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithLastUpdatedBy(Guid.NewGuid())
                .WithLastUpdatedByName(Guid.NewGuid().ToString())
                .WithLastUpdated(new DateTime(2020, 06, 29))
                .WithOrderItem(orderItem)
                .Build();

            var expected = new
            {
                order.LastUpdatedBy,
                order.LastUpdatedByName,
                order.LastUpdated
            };

            order.UpdateOrderItem(
                unknownOrderItemId,
                DateTime.UtcNow.AddDays(1),
                orderItem.Quantity + 1,
                TimeUnit.PerMonth,
                orderItem.Price + 1m,
                Guid.NewGuid(),
                Guid.NewGuid().ToString());

            order.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void CalculateCostPerYear_OrderItemCostTypeRecurring_ReturnsTotalOrderItemCost()
        {
            const int orderItemId1 = 1;
            const int orderItemId2 = 1;

            var orderItem1 = OrderItemBuilder
                .Create()
                .WithOrderItemId(orderItemId1)
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .WithProvisioningType(ProvisioningType.Declarative)
                .WithPrice(120)
                .WithQuantity(2)
                .Build();

            var orderItem2 = OrderItemBuilder
                .Create()
                .WithOrderItemId(orderItemId2)
                .WithCatalogueItemType(CatalogueItemType.AdditionalService)
                .WithProvisioningType(ProvisioningType.Patient)
                .WithPrice(240)
                .WithQuantity(2)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem1)
                .WithOrderItem(orderItem2)
                .Build();

            order.CalculateCostPerYear(CostType.Recurring).Should().Be(2880);
        }

        [Test]
        public void CalculateCostPerYear_OrderItemCostTypeOneOff_ReturnsZero()
        {
            const int orderItemId = 1;

            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(orderItemId)
                .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                .WithProvisioningType(ProvisioningType.Declarative)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .Build();

            order.CalculateCostPerYear(CostType.Recurring).Should().Be(0);
        }
    }
}
