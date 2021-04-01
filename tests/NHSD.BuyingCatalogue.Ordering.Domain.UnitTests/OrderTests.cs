using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderTests
    {
        [Test]
        public static void AddOrUpdateOrderItem_NullOrderItem_ThrowsArgumentNullException()
        {
            var order = OrderBuilder.Create().Build();

            Assert.Throws<ArgumentNullException>(() => order.AddOrUpdateOrderItem(null));
        }

        [Test]
        public static void AddOrUpdateOrderItem_OrderItem_ItemAdded()
        {
            var order = OrderBuilder.Create().Build();
            var orderItem = OrderItemBuilder.Create().Build();

            order.AddOrUpdateOrderItem(orderItem);

            var expected = new List<OrderItem> { orderItem };
            order.OrderItems.Should().BeEquivalentTo(expected);
        }

        [TestCase(CatalogueItemType.Solution, true)]
        [TestCase(CatalogueItemType.AdditionalService, false)]
        [TestCase(CatalogueItemType.AssociatedService, false)]
        public static void AddOrderItem_OrderItem_CatalogueItemType_CatalogueSolutionsViewedMatchExpectedValue(
            CatalogueItemType catalogueItemType,
            bool expectedInput)
        {
            var order = OrderBuilder
                .Create()
                .WithCatalogueSolutionsViewed(false)
                .Build();

            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = catalogueItemType })
                .Build();

            order.AddOrUpdateOrderItem(orderItem);

            order.Progress.CatalogueSolutionsViewed.Should().Be(expectedInput);
        }

        [TestCase(CatalogueItemType.Solution, false)]
        [TestCase(CatalogueItemType.AdditionalService, true)]
        [TestCase(CatalogueItemType.AssociatedService, false)]
        public static void AddOrderItem_OrderItem_CatalogueItemType_AdditionalServicesViewedMatchExpectedValue(
            CatalogueItemType catalogueItemType,
            bool expectedInput)
        {
            var order = OrderBuilder
                .Create()
                .WithAdditionalServicesViewed(false)
                .Build();

            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = catalogueItemType })
                .Build();

            order.AddOrUpdateOrderItem(orderItem);

            order.Progress.AdditionalServicesViewed.Should().Be(expectedInput);
        }

        [TestCase(CatalogueItemType.Solution, false)]
        [TestCase(CatalogueItemType.AdditionalService, true)]
        [TestCase(CatalogueItemType.AssociatedService, false)]
        public static void AddOrderItem_OrderItem_CatalogueItemType_AssociatedServicesViewedMatchExpectedValue(
            CatalogueItemType catalogueItemType,
            bool expectedInput)
        {
            var order = OrderBuilder
                .Create()
                .WithAdditionalServicesViewed(false)
                .Build();

            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = catalogueItemType })
                .Build();

            order.AddOrUpdateOrderItem(orderItem);

            order.Progress.AdditionalServicesViewed.Should().Be(expectedInput);
        }

        [Test]
        public static void AddOrderItem_AddSameOrderItem_ReturnsOneOrderItem()
        {
            var order = OrderBuilder.Create().Build();
            var orderItem = OrderItemBuilder.Create().Build();

            order.AddOrUpdateOrderItem(orderItem);
            order.AddOrUpdateOrderItem(orderItem);

            var expected = new List<OrderItem> { orderItem };
            order.OrderItems.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static void AddOrderItem_AddDifferentOrderItem_ReturnsTwoOrderItems()
        {
            var order = OrderBuilder.Create().Build();
            var orderItem = OrderItemBuilder.Create()
                .WithCatalogueItem(new CatalogueItem { Id = new CatalogueItemId(1, "1"), CatalogueItemType = CatalogueItemType.Solution })
                .Build();

            var orderItemSecond = OrderItemBuilder.Create().Build();

            order.AddOrUpdateOrderItem(orderItem);
            order.AddOrUpdateOrderItem(orderItemSecond);

            var expected = new List<OrderItem> { orderItem, orderItemSecond };
            order.OrderItems.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static void SetServiceRecipient_NullRecipients_ThrowsException()
        {
            var order = OrderBuilder.Create().Build();

            Assert.Throws<ArgumentNullException>(() => order.SetSelectedServiceRecipients(null));
        }

        [Test]
        public static void CalculateCostPerYear_Recurring_OrderItemCostTypeRecurring_ReturnsTotalOrderItemCost()
        {
            var order = new Order();
            var orderItem1 = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { Id = new CatalogueItemId(1, "1"), CatalogueItemType = CatalogueItemType.Solution })
                .WithProvisioningType(ProvisioningType.Declarative)
                .WithPrice(120)
                .WithRecipient(new OrderItemRecipient { Quantity = 2 })
                .Build();

            var orderItem2 = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AdditionalService })
                .WithProvisioningType(ProvisioningType.Patient)
                .WithPrice(240)
                .WithRecipient(new OrderItemRecipient { Quantity = 2 })
                .Build();

            order.AddOrUpdateOrderItem(orderItem1);
            order.AddOrUpdateOrderItem(orderItem2);

            order.CalculateCostPerYear(CostType.Recurring).Should().Be(8640);
        }

        [Test]
        public static void CalculateCostPerYear_Recurring_OrderItemCostTypeOneOff_ReturnsZero()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AssociatedService })
                .WithProvisioningType(ProvisioningType.Declarative)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .Build();

            order.CalculateCostPerYear(CostType.Recurring).Should().Be(0);
        }

        [Test]
        public static void CalculateCostPerYear_OneOff_OrderItemCostTypeOneOff_ReturnsTotalOneOffCost()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AssociatedService })
                .WithProvisioningType(ProvisioningType.Declarative)
                .WithPrice(5)
                .WithRecipient(new OrderItemRecipient { Quantity = 10 })
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
        public static void CalculateCostPerYear_OneOff_OrderItemCostTypeRecurring_ReturnsZero()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AssociatedService })
                .WithProvisioningType(ProvisioningType.OnDemand)
                .WithPrice(5)
                .WithRecipient(new OrderItemRecipient { Quantity = 10 })
                .Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .Build();

            order.CalculateCostPerYear(CostType.OneOff).Should().Be(0);
        }

        [TestCase(CatalogueItemType.AssociatedService, ProvisioningType.OnDemand, 10, 2, 720)]
        [TestCase(CatalogueItemType.AssociatedService, ProvisioningType.Declarative, 20, 4, 960)]
        public static void CalculateTotalOwnershipCost_SingleOneOffOrRecurringOrderItem_ReturnsTotalOwnershipCost(
            CatalogueItemType catalogueItemType,
            ProvisioningType provisioningType,
            decimal price,
            int quantity,
            decimal totalOwnershipCost)
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = catalogueItemType })
                .WithProvisioningType(provisioningType)
                .WithPrice(price)
                .WithRecipient(new OrderItemRecipient { Quantity = quantity })
                .Build();

            var order = OrderBuilder
                .Create()
                .WithOrderItem(orderItem)
                .Build();

            order.CalculateTotalOwnershipCost().Should().Be(totalOwnershipCost);
        }

        [Test]
        public static void CalculateTotalOwnershipCost_RecurringAndOneOff_ReturnsTotalOwnershipCost()
        {
            var order = new Order();

            var orderItem1 = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { Id = new CatalogueItemId(1, "1"), CatalogueItemType = CatalogueItemType.AssociatedService })
                .WithProvisioningType(ProvisioningType.OnDemand)
                .WithPrice(5)
                .WithRecipient(new OrderItemRecipient { Quantity = 10 })
                .Build();

            var orderItem2 = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AdditionalService })
                .WithProvisioningType(ProvisioningType.Patient)
                .WithPrice(240)
                .WithRecipient(new OrderItemRecipient { Quantity = 2 })
                .Build();

            order.AddOrUpdateOrderItem(orderItem1);
            order.AddOrUpdateOrderItem(orderItem2);

            order.CalculateTotalOwnershipCost().Should().Be(19080);
        }

        [Test]
        public static void CalculateTotalOwnershipCost_NoOrderItemsExist_ReturnsZero()
        {
            var order = OrderBuilder
                .Create()
                .Build();

            order.CalculateTotalOwnershipCost().Should().Be(0);
        }

        [Ignore("Work in progress")]
        [TestCase(null, false, false, false, false, false, false, false)]
        [TestCase(true, true, true, true, true, true, false, true)]
        [TestCase(true, true, true, true, true, true, true, true)]
        [TestCase(true, true, true, true, false, false, true, false)]
        [TestCase(true, true, true, true, true, false, true, true)]
        [TestCase(true, true, false, false, true, false, false, false)]
        [TestCase(true, true, false, true, true, true, false, false)]
        [TestCase(true, false, true, false, false, false, true, false)]
        public static void CanComplete_ReturnsCorrectResult(
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

            if (hasSolution)
            {
                var itemBuilder = OrderItemBuilder
                    .Create()
                    .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.Solution });

                if (hasRecipient)
                    itemBuilder.WithRecipient(new OrderItemRecipient());

                orderBuilder.WithOrderItem(itemBuilder.Build());
            }

            if (hasAssociated)
            {
                var itemBuilder = OrderItemBuilder
                    .Create()
                    .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AssociatedService });

                if (hasRecipient)
                    itemBuilder.WithRecipient(new OrderItemRecipient());

                orderBuilder.WithOrderItem(itemBuilder.Build());
            }

            var order = orderBuilder.Build();

            order.CanComplete().Should().Be(expectedResult);
        }

        [Ignore("Work in progress")]
        [Test]
        public static void Complete_CanCompleteOrder_ReturnsSuccessfulResult()
        {
            var order = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(true)
                .WithServiceRecipientsViewed(true)
                .WithAssociatedServicesViewed(true)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AssociatedService })
                    .WithRecipient(new OrderItemRecipient())
                    .Build()).Build();

            var actual = order.Complete();

            actual.Should().BeEquivalentTo(Result.Success());
        }

        [Ignore("Work in progress")]
        [Test]
        public static void Complete_CanCompleteOrder_ReturnsCompleteOrderStatus()
        {
            var order = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(true)
                .WithServiceRecipientsViewed(true)
                .WithAssociatedServicesViewed(true)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AssociatedService })
                    .WithRecipient(new OrderItemRecipient())
                    .Build()).Build();

            order.OrderStatus.Should().Be(OrderStatus.Incomplete);

            order.Complete();

            order.OrderStatus.Should().Be(OrderStatus.Complete);
        }

        [Ignore("Work in progress")]
        [Test]
        public static void Complete_CanCompleteOrder_CompletedDateIsUpdated()
        {
            var order = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(true)
                .WithServiceRecipientsViewed(true)
                .WithAssociatedServicesViewed(true)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AssociatedService })
                    .WithRecipient(new OrderItemRecipient())
                    .Build()).Build();

            order.Completed.Should().BeNull();

            order.Complete();

            order.Completed.Should().NotBeNull();
        }

        [Test]
        public static void Complete_CanNotCompleteOrder_ReturnsFailedResult()
        {
            var order = OrderBuilder
                .Create()
                .Build();

            var actual = order.Complete();

            actual.Should().Be(Result.Failure(OrderErrors.OrderNotComplete()));
        }

        [Test]
        public static void Complete_CanNotCompleteOrder_ReturnsIncompleteOrderStatus()
        {
            var order = OrderBuilder
                .Create()
                .Build();

            order.Complete();

            order.OrderStatus.Should().Be(OrderStatus.Incomplete);
        }

        [Test]
        [CommonAutoData]
        public static void DeleteOrderItem_OrderItemPresent_DeletesOrderItem(
            OrderItem orderItem1,
            OrderItem orderItem2,
            OrderItem orderItem3,
            Order order)
        {
            orderItem3.CatalogueItem.ParentCatalogueItemId = orderItem1.CatalogueItem.Id;
            order.AddOrUpdateOrderItem(orderItem1);
            order.AddOrUpdateOrderItem(orderItem2);
            order.AddOrUpdateOrderItem(orderItem3);
            order.OrderItems.Count.Should().Be(3);

            order.DeleteOrderItem(orderItem1.CatalogueItem.Id);

            order.OrderItems.Count.Should().Be(1);
        }

        [Test]
        [CommonAutoData]
        public static void DeleteOrderItem_OrderItemPresent_ReturnsNumberOfItemsDeleted(
            OrderItem orderItem1,
            OrderItem orderItem2,
            OrderItem orderItem3,
            Order order)
        {
            orderItem3.CatalogueItem.ParentCatalogueItemId = orderItem1.CatalogueItem.Id;
            order.AddOrUpdateOrderItem(orderItem1);
            order.AddOrUpdateOrderItem(orderItem2);
            order.AddOrUpdateOrderItem(orderItem3);
            order.OrderItems.Count.Should().Be(3);

            var actual = order.DeleteOrderItem(orderItem1.CatalogueItem.Id);

            actual.Should().Be(2);
        }

        [Test]
        [CommonAutoData]
        public static void DeleteOrderItem_NoOrderItem_ReturnsZero(Order order)
        {
            order.OrderItems.Count.Should().Be(0);

            var actual = order.DeleteOrderItem(default(CatalogueItemId));

            actual.Should().Be(0);
        }

        [Test]
        [CommonAutoData]
        public static void HasSolution_HasSolutionItem_ReturnsTrue(Order order)
        {
            order.AddOrUpdateOrderItem(OrderItemBuilder.Create()
                .WithOrderId(42)
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.Solution })
                .Build());

            var actual = order.HasSolution();

            actual.Should().BeTrue();
        }

        [Test]
        [CommonAutoData]
        public static void HasSolution_HasNoSolutionItem_ReturnsFalse(Order order)
        {
            var actual = order.HasSolution();

            actual.Should().BeFalse();
        }

        [Test]
        [CommonAutoData]
        public static void SetDefaultDeliveryDate_AddsNewDate(Order order)
        {
            var item = new OrderItem
            {
                CatalogueItem = new CatalogueItem
                {
                    CatalogueItemType = CatalogueItemType.Solution,
                },
            };

            order.AddOrUpdateOrderItem(item);

            order.DefaultDeliveryDates.Should().BeEmpty();

            order.SetDefaultDeliveryDate(item.CatalogueItem.Id, DateTime.Today);

            order.DefaultDeliveryDates.Should().HaveCount(1);

            var expectedDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = item.CatalogueItem.Id,
                DeliveryDate = DateTime.Today,
                OrderId = order.Id,
            };

            var actualDefaultDeliveryDate = order.DefaultDeliveryDates[0];

            actualDefaultDeliveryDate.Should().BeEquivalentTo(expectedDefaultDeliveryDate);
        }

        [Test]
        [CommonAutoData]
        public static void SetDefaultDeliveryDate_UpdatesExistingDate(Order order)
        {
            var item = new OrderItem
            {
                CatalogueItem = new CatalogueItem
                {
                    CatalogueItemType = CatalogueItemType.Solution,
                },
            };

            var existingDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = item.CatalogueItem.Id,
                DeliveryDate = DateTime.Today,
                OrderId = order.Id,
            };

            order.AddOrUpdateOrderItem(item);
            BackingField.AddListItem(order, nameof(Order.DefaultDeliveryDates), existingDefaultDeliveryDate);

            var tomorrow = DateTime.Today.AddDays(1);
            order.SetDefaultDeliveryDate(item.CatalogueItem.Id, tomorrow);

            order.DefaultDeliveryDates.Should().HaveCount(1);

            var expectedDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = item.CatalogueItem.Id,
                DeliveryDate = tomorrow,
                OrderId = order.Id,
            };

            var actualDefaultDeliveryDate = order.DefaultDeliveryDates[0];

            actualDefaultDeliveryDate.Should().BeEquivalentTo(expectedDefaultDeliveryDate);
        }

        [Test]
        [CommonAutoData]
        public static void SetSelectedServiceRecipient_SelectedRecipientsIsNull_ThrowsException(Order order)
        {
            Assert.Throws<ArgumentNullException>(() => order.SetSelectedServiceRecipients(null));
        }

        [Test]
        [CommonAutoData]
        public static void SetSelectedServiceRecipient_AddsSelectedRecipients(
            Order order,
            IReadOnlyList<SelectedServiceRecipient> recipients)
        {
            order.SelectedServiceRecipients.Should().BeEmpty();

            order.SetSelectedServiceRecipients(recipients);

            order.SelectedServiceRecipients.Should().BeEquivalentTo(recipients);
        }

        [Test]
        [CommonAutoData]
        public static void SetSelectedServiceRecipient_ReplacesSelectedRecipients(
            Order order,
            IReadOnlyList<SelectedServiceRecipient> initialRecipients,
            IReadOnlyList<SelectedServiceRecipient> updatedRecipients)
        {
            BackingField.AddListItems(order, nameof(Order.SelectedServiceRecipients), initialRecipients);
            order.SelectedServiceRecipients.Should().BeEquivalentTo(initialRecipients);

            order.SetSelectedServiceRecipients(updatedRecipients);

            order.SelectedServiceRecipients.Should().BeEquivalentTo(updatedRecipients);
        }

        [Test]
        [CommonAutoData]
        public static void SetSelectedServiceRecipient_UpdatesServiceRecipientsViewed(
            Order order,
            IReadOnlyList<SelectedServiceRecipient> recipients)
        {
            order.Progress.ServiceRecipientsViewed.Should().BeFalse();

            order.SetSelectedServiceRecipients(recipients);

            order.Progress.ServiceRecipientsViewed.Should().BeTrue();
        }

        [Test]
        [CommonAutoData]
        public static void SetSelectedServiceRecipient_NoRecipients_UpdatesCatalogueSolutionsViewed(Order order)
        {
            order.Progress.CatalogueSolutionsViewed = true;

            order.SetSelectedServiceRecipients(Array.Empty<SelectedServiceRecipient>());

            order.Progress.CatalogueSolutionsViewed.Should().BeFalse();
        }

        [Test]
        [CommonAutoData]
        public static void SetLastUpdatedBy_SetsUserId(Order order, Guid userId, string userName)
        {
            order.SetLastUpdatedBy(userId, userName);

            order.LastUpdatedBy.Should().Be(userId);
        }

        [Test]
        [CommonAutoData]
        public static void SetLastUpdatedBy_SetsUserName(Order order, Guid userId, string userName)
        {
            order.SetLastUpdatedBy(userId, userName);

            order.LastUpdatedByName.Should().Be(userName);
        }

        [Test]
        [CommonAutoData]
        public static void SetLastUpdatedBy_NullUserName_ThrowsException(Order order, Guid userId)
        {
            Assert.Throws<ArgumentNullException>(() => order.SetLastUpdatedBy(userId, null));
        }

        [Test]
        [CommonAutoData]
        public static void SetLastUpdatedBy_SetsLastUpdate(Order order, Guid userId, string userName)
        {
            var yesterday = DateTime.Today.AddDays(-1);
            BackingField.SetValue(order, nameof(Order.LastUpdated), yesterday);
            order.LastUpdated.Should().Be(yesterday);

            order.SetLastUpdatedBy(userId, userName);

            order.LastUpdated.Should().BeAfter(DateTime.Today);
        }

        [Test]
        [CommonAutoData]
        public static void Equals_Order_OtherIsNull_ReturnsFalse(Order order)
        {
            var isEqual = order.Equals(null);

            isEqual.Should().BeFalse();
        }

        [Test]
        [CommonAutoData]
        public static void Equals_Order_OtherIsThis_ReturnsTrue(Order order)
        {
            var isEqual = order.Equals(order);

            isEqual.Should().BeTrue();
        }

        [Test]
        [CommonAutoData]
        public static void Equals_Order_OtherHasSameId_ReturnsTrue(Order order, Order other)
        {
            BackingField.SetValue(order, nameof(Order.Id), 1);
            BackingField.SetValue(other, nameof(Order.Id), 1);

            var isEqual = order.Equals(other);

            isEqual.Should().BeTrue();
        }

        [Test]
        [CommonAutoData]
        public static void Equals_Object_OtherHasSameId_ReturnsTrue(Order order, Order other)
        {
            BackingField.SetValue(order, nameof(Order.Id), 1);
            BackingField.SetValue(other, nameof(Order.Id), 1);

            var isEqual = order.Equals((object)other);

            isEqual.Should().BeTrue();
        }

        [Test]
        [CommonAutoData]
        public static void Equals_Object_DifferentType_ReturnsFalse(Order order)
        {
            BackingField.SetValue(order, nameof(Order.Id), 1);
            var other = new { Id = 1 };

            var isEqual = order.Equals(other);

            isEqual.Should().BeFalse();
        }

        [Test]
        [CommonAutoData]
        public static void GetHashCode_ReturnsExpectedValue(Order order)
        {
            BackingField.SetValue(order, nameof(Order.Id), 1);

            var hash = order.GetHashCode();

            hash.Should().Be(order.Id.GetHashCode());
        }
    }
}
