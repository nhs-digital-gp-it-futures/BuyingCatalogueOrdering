﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
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

        [TestCase(nameof(CatalogueItemType.Solution), true)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), false)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), false)]
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

        [TestCase(nameof(CatalogueItemType.Solution), false)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), true)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), false)]
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

        [TestCase(nameof(CatalogueItemType.Solution), false)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), true)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), false)]
        public void AddOrderItem_OrderItem_CatalogueItemType_AssociatedServicesViewedMatchExpectedValue(
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
        public void CalculateCostPerYear_Recurring_OrderItemCostTypeRecurring_ReturnsTotalOrderItemCost()
        {
            const int orderItemId1 = 1;
            const int orderItemId2 = 2;

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

            order.CalculateCostPerYear(CostType.Recurring).Should().Be(8640);
        }

        [Test]
        public void CalculateCostPerYear_Recurring_OrderItemCostTypeOneOff_ReturnsZero()
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

        [Test]
        public void CalculateCostPerYear_OneOff_OrderItemCostTypeOneOff_ReturnsTotalOneOffCost()
        {
            const int orderItemId = 1;

            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(orderItemId)
                .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                .WithProvisioningType(ProvisioningType.Declarative)
                .WithPrice(5)
                .WithQuantity(10)
                .WithEstimationPeriod(null)
                .WithPriceTimeUnit(null)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .Build();

            order.CalculateCostPerYear(CostType.OneOff).Should().Be(50);
        }

        [Test]
        public void CalculateCostPerYear_OneOff_OrderItemCostTypeRecurring_ReturnsZero()
        {
            const int orderItemId = 1;

            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(orderItemId)
                .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                .WithProvisioningType(ProvisioningType.OnDemand)
                .WithPrice(5)
                .WithQuantity(10)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .Build();

            order.CalculateCostPerYear(CostType.OneOff).Should().Be(0);
        }

        [TestCase(nameof(CatalogueItemType.AssociatedService), nameof(ProvisioningType.OnDemand), 10, 2, 720)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), nameof(ProvisioningType.Declarative), 20, 4, 960)]
        public void CalculateTotalOwnershipCost_SingleOneOffOrRecurringOrderItem_ReturnsTotalOwnershipCost(string catalogueItemTypeName, string provisioningTypeName, decimal price, int quantity, decimal totalOwnershipCost)
        {
            const int orderItemId = 1;

            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(orderItemId)
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeName))
                .WithProvisioningType(ProvisioningType.FromName(provisioningTypeName))
                .WithPrice(price)
                .WithQuantity(quantity)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .Build();

            order.CalculateTotalOwnershipCost().Should().Be(totalOwnershipCost);
        }

        [Test]
        public void CalculateTotalOwnershipCost_RecurringAndOneOff_ReturnsTotalOwershipCost()
        {
            const int orderItemId1 = 1;
            const int orderItemId2 = 2;

            var orderItem1 = OrderItemBuilder
                .Create()
                .WithOrderItemId(orderItemId1)
                .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                .WithProvisioningType(ProvisioningType.OnDemand)
                .WithPrice(5)
                .WithQuantity(10)
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

            order.CalculateTotalOwnershipCost().Should().Be(19080);
        }

        [Test]
        public void CalculateTotalOwnershipCost_NoOrderItemsExist_ReturnsZero()
        {
            var order = OrderBuilder
                .Create()
                .Build();

            order.CalculateTotalOwnershipCost().Should().Be(0);
        }

        [TestCase(null, false, false, false, false, false, false, false)]
        [TestCase(true, true, true, true, true, true, false, true)]
        [TestCase(true, true, true, true, true, true, true, true)]
        [TestCase(true, true, true, true, false, false, true, true)]
        [TestCase(true, true, true, true, true, false, true, true)]
        [TestCase(true, true, false, false, true, false, false, false)]
        [TestCase(true, true, false, true, true, true, false, false)]
        [TestCase(true, false, true, false, false, false, true, false)]
        public void CanComplete_ReturnsCorrectResult(
            bool? fundingComplete,
            bool recipientViewed,
            bool associatedViewed,
            bool solutionViewed,
            bool hasRecipient,
            bool hasSolution,
            bool hasAssociated,
            bool expectedResult)
        {
            var orderBuilder = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(fundingComplete)
                .WithServiceRecipientsViewed(recipientViewed)
                .WithAssociatedServicesViewed(associatedViewed)
                .WithCatalogueSolutionsViewed(solutionViewed);

            if (hasRecipient)
            {
                orderBuilder.WithServiceRecipient(("ODS1", "Some service recipient"));
            }

            if (hasSolution)
            {
                orderBuilder.WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .Build());
            }

            if (hasAssociated)
            {
                orderBuilder.WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                    .Build());
            }

            var order = orderBuilder.Build();

            order.CanComplete().Should().Be(expectedResult);
        }

        [Test]
        public void Complete_CanCompleteOrder_ReturnsSuccessfulResult()
        {
            var order = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(true)
                .WithServiceRecipientsViewed(true)
                .WithAssociatedServicesViewed(true)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                    .Build()).Build();
            
            var actual = order.Complete(Guid.Empty, string.Empty);

            actual.Should().BeEquivalentTo(Result.Success());
        }

        [Test]
        public void Complete_CanCompleteOrder_ReturnsCompleteOrderStatus()
        {
            var order = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(true)
                .WithServiceRecipientsViewed(true)
                .WithAssociatedServicesViewed(true)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                    .Build()).Build();

            order.OrderStatus.Should().Be(OrderStatus.Incomplete);

            order.Complete(Guid.Empty, string.Empty);

            order.OrderStatus.Should().Be(OrderStatus.Complete);
        }

        [Test]
        public void Complete_CanCompleteOrder_CompletedDateIsUpdated()
        {
            var order = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(true)
                .WithServiceRecipientsViewed(true)
                .WithAssociatedServicesViewed(true)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                    .Build()).Build();

            order.Completed.Should().BeNull();

            order.Complete(Guid.Empty, string.Empty);

            order.Completed.Should().NotBeNull();
        }

        [Test]
        public void Complete_CanCompleteOrder_LastUpdatedByChanged()
        {
            var lastUpdatedBy = Guid.NewGuid();

            var order = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(true)
                .WithServiceRecipientsViewed(true)
                .WithAssociatedServicesViewed(true)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                    .Build()).Build();

            order.LastUpdatedBy.Should().NotBe(lastUpdatedBy);

            order.Complete(lastUpdatedBy, String.Empty);

            order.LastUpdatedBy.Should().Be(lastUpdatedBy);
        }

        [Test]
        public void Complete_CanCompleteOrder_LastUpdatedByNameChanged()
        {
            var lastUpdatedByName = Guid.NewGuid().ToString();

            var order = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(true)
                .WithServiceRecipientsViewed(true)
                .WithAssociatedServicesViewed(true)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                    .Build()).Build();

            order.LastUpdatedByName.Should().NotBe(lastUpdatedByName);

            order.Complete(Guid.Empty, lastUpdatedByName);

            order.LastUpdatedByName.Should().Be(lastUpdatedByName);
        }

        [Test]
        public void Complete_CanNotCompleteOrder_ReturnsFailedResult()
        {
            var order = OrderBuilder
                .Create()
                .Build();

            var actual = order.Complete(Guid.Empty, string.Empty);

            actual.Should().Be(Result.Failure(OrderErrors.OrderNotComplete()));
        }

        [Test]
        public void Complete_CanNotCompleteOrder_ReturnsIncompleteOrderStatus()
        {
            var order = OrderBuilder
                .Create()
                .Build();

            order.Complete(Guid.Empty, string.Empty);

            order.OrderStatus.Should().Be(OrderStatus.Incomplete);
        }
    }
}
