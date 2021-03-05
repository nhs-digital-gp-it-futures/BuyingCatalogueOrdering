using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrderItemTests
    {
        [Test]
        public static void MarkOrderSectionAsViewed_NullOrder_ThrowsArgumentNullException()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.Solution })
                .Build();

            Assert.Throws<ArgumentNullException>(() => orderItem.MarkOrderSectionAsViewed(null));
        }

        [Test]
        public static void MarkOrderSectionAsViewed_Order_CatalogueSolutionsViewedIsTrue()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.Solution })
                .Build();

            var order = OrderBuilder
                .Create()
                .WithCatalogueSolutionsViewed(false)
                .Build();

            orderItem.MarkOrderSectionAsViewed(order);

            order.Progress.CatalogueSolutionsViewed.Should().BeTrue();
        }

        [Test]
        public static void MarkOrderSectionAsViewed_Order_AdditionalServicesViewedIsTrue()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = CatalogueItemType.AdditionalService })
                .Build();

            var order = OrderBuilder
                .Create()
                .WithCatalogueSolutionsViewed(false)
                .Build();

            orderItem.MarkOrderSectionAsViewed(order);

            order.Progress.AdditionalServicesViewed.Should().BeTrue();
        }

        [Test]
        public static void Equals_DifferentType_ReturnsFalse()
        {
            var item = OrderItemBuilder.Create().Build();

            var anonItem = new
            {
                Order = item.OrderId,
                item.CatalogueItem,
            };

            var isEqual = item.Equals(anonItem);

            isEqual.Should().BeFalse();
        }

        [Test]
        public static void Equals_NullOrderItem_AreNotEqual()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .Build();

            orderItem.Equals(null).Should().BeFalse();
        }

        [Test]
        public static void Equals_SameOrderItemReference_AreEqual()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .Build();

            orderItem.Equals(orderItem).Should().BeTrue();
        }

        [Test]
        public static void Equals_DifferentOrderId_AreNotEqual()
        {
            var orderItem1 = OrderItemBuilder.Create().Build();
            var orderItem2 = OrderItemBuilder.Create().WithOrderId(2).Build();

            orderItem1.Equals(orderItem2).Should().BeFalse();
        }

        [Test]
        public static void CalculateCost_WithPriceTypePerMonth_CalculatesCostCorrectly()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithPrice(1m)
                .WithRecipient(new OrderItemRecipient { Quantity = 10 })
                .WithPriceTimeUnit(TimeUnit.PerMonth)
                .Build();

            orderItem.CalculateTotalCostPerYear().Should().Be(120);
        }

        [Test]
        public static void CalculateTotalCostPerYear_WithPriceTypePerYear_CalculatesCostCorrectly()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithPrice(1m)
                .WithRecipient(new OrderItemRecipient { Quantity = 10 })
                .WithPriceTimeUnit(TimeUnit.PerYear)
                .Build();

            orderItem.CalculateTotalCostPerYear().Should().Be(10);
        }

        [Test]
        public static void CalculateTotalCostPerYear_WithEstimationPeriodPerMonth_CalculatesCorrectly()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithPrice(0.1m)
                .WithRecipient(new OrderItemRecipient { Quantity = 10 })
                .WithPriceTimeUnit(null)
                .WithEstimationPeriod(TimeUnit.PerMonth)
                .Build();

            orderItem.CalculateTotalCostPerYear().Should().Be(12);
        }

        [Test]
        public static void CalculateTotalCostPerYear_WithEstimationPeriodPerYear_CalculatesCorrectly()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithPrice(0.1m)
                .WithRecipient(new OrderItemRecipient { Quantity = 10 })
                .WithPriceTimeUnit(null)
                .WithEstimationPeriod(TimeUnit.PerYear)
                .Build();

            orderItem.CalculateTotalCostPerYear().Should().Be(1);
        }

        [Test]
        public static void CalculateTotalCostPerYear_WithNoPriceTypeOrEstimationPeriod_PriceIsPriceTimesQuantity()
        {
            const int price = 26;
            const int quantity = 13;

            var orderItem = OrderItemBuilder
                .Create()
                .WithPrice(price)
                .WithRecipient(new OrderItemRecipient { Quantity = quantity })
                .WithPriceTimeUnit(null)
                .WithEstimationPeriod(null)
                .Build();

            orderItem.CalculateTotalCostPerYear().Should().Be(price * quantity);
        }

        [Test]
        public static void CostType_OrderItemIsOneOff_ReturnsCorrectCostType()
        {
            var orderItem = CreateOrderItem(CatalogueItemType.AssociatedService, ProvisioningType.Declarative);
            orderItem.CostType.Should().Be(CostType.OneOff);
        }

        [TestCase(CatalogueItemType.Solution, ProvisioningType.Declarative, CostType.Recurring)]
        [TestCase(CatalogueItemType.Solution, ProvisioningType.OnDemand, CostType.Recurring)]
        [TestCase(CatalogueItemType.Solution, ProvisioningType.Patient, CostType.Recurring)]
        [TestCase(CatalogueItemType.AdditionalService, ProvisioningType.Declarative, CostType.Recurring)]
        [TestCase(CatalogueItemType.AdditionalService, ProvisioningType.OnDemand, CostType.Recurring)]
        [TestCase(CatalogueItemType.AdditionalService, ProvisioningType.Patient, CostType.Recurring)]
        [TestCase(CatalogueItemType.AssociatedService, ProvisioningType.OnDemand, CostType.Recurring)]
        [TestCase(CatalogueItemType.AssociatedService, ProvisioningType.Patient, CostType.Recurring)]
        [TestCase(CatalogueItemType.AssociatedService, ProvisioningType.Declarative, CostType.OneOff)]
        public static void CostType_DeterminesTheCostType_ReturnsCorrectCostType(CatalogueItemType catalogueItemType, ProvisioningType provisioningType, CostType costType)
        {
            var orderItem = CreateOrderItem(catalogueItemType, provisioningType);

            orderItem.CostType.Should().Be(costType);
        }

        [Test]
        [CommonAutoData]
        public static void GetHashCode_EqualItems_ReturnsExpectedValue(int orderId, CatalogueItem catalogueItem)
        {
            var orderItem1 = new OrderItem { OrderId = orderId, CatalogueItem = catalogueItem };
            var orderItem2 = new OrderItem { OrderId = orderId, CatalogueItem = catalogueItem };

            var hash1 = orderItem1.GetHashCode();
            var hash2 = orderItem2.GetHashCode();

            hash1.Should().Be(hash2);
        }

        [Test]
        [CommonAutoData]
        public static void GetHashCode_DifferentOrderId_ReturnsExpectedValue(
            int orderId1,
            int orderId2,
            CatalogueItem catalogueItem)
        {
            var orderItem1 = new OrderItem { OrderId = orderId1, CatalogueItem = catalogueItem };
            var orderItem2 = new OrderItem { OrderId = orderId2, CatalogueItem = catalogueItem };

            var hash1 = orderItem1.GetHashCode();
            var hash2 = orderItem2.GetHashCode();

            hash1.Should().NotBe(hash2);
        }

        [Test]
        [CommonAutoData]
        public static void GetHashCode_DifferentCatalogueItem_ReturnsExpectedValue(
            int orderId,
            CatalogueItem catalogueItem1,
            CatalogueItem catalogueItem2)
        {
            var orderItem1 = new OrderItem { OrderId = orderId, CatalogueItem = catalogueItem1 };
            var orderItem2 = new OrderItem { OrderId = orderId, CatalogueItem = catalogueItem2 };

            var hash1 = orderItem1.GetHashCode();
            var hash2 = orderItem2.GetHashCode();

            hash1.Should().NotBe(hash2);
        }

        private static OrderItem CreateOrderItem(CatalogueItemType catalogueItemType, ProvisioningType provisioningType)
        {
            return OrderItemBuilder
                .Create()
                .WithCatalogueItem(new CatalogueItem { CatalogueItemType = catalogueItemType })
                .WithProvisioningType(provisioningType)
                .Build();
        }
    }
}
