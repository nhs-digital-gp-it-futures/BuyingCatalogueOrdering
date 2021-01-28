using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrderItemTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public static void Constructor_NullOrEmptyCatalogueItemId_ThrowsArgumentException(string catalogueItemIdInput)
        {
            void Test()
            {
                OrderItemBuilder
                    .Create()
                    .WithCatalogueItemId(catalogueItemIdInput)
                    .Build();
            }

            Assert.Throws<ArgumentException>(Test);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public static void Constructor_NullOrEmptyCurrencyCode_ThrowsArgumentException(string currencyCodeInput)
        {
            void Test()
            {
                OrderItemBuilder
                    .Create()
                    .WithCurrencyCode(currencyCodeInput)
                    .Build();
            }

            Assert.Throws<ArgumentException>(Test);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public static void Constructor_NullOrEmptyCatalogueItemName_ThrowsArgumentException(string catalogueItemNameInput)
        {
            void Test()
            {
                OrderItemBuilder
                    .Create()
                    .WithCatalogueItemName(catalogueItemNameInput)
                    .Build();
            }

            Assert.Throws<ArgumentException>(Test);
        }

        [Test]
        public static void Constructor_NullCataloguePriceUnit_ThrowsArgumentNullException()
        {
            static void Test()
            {
                OrderItemBuilder
                    .Create()
                    .WithCataloguePriceUnit(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public static void MarkOrderSectionAsViewed_NullOrder_ThrowsArgumentNullException()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .Build();

            Assert.Throws<ArgumentNullException>(() => orderItem.MarkOrderSectionAsViewed(null));
        }

        [Test]
        public static void MarkOrderSectionAsViewed_Order_CatalogueSolutionsViewedIsTrue()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithCatalogueSolutionsViewed(false)
                .Build();

            orderItem.MarkOrderSectionAsViewed(order);

            order.CatalogueSolutionsViewed.Should().BeTrue();
        }

        [Test]
        public static void MarkOrderSectionAsViewed_Order_AdditionalServicesViewedIsTrue()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.AdditionalService)
                .Build();

            var order = OrderBuilder
                .Create()
                .WithCatalogueSolutionsViewed(false)
                .Build();

            orderItem.MarkOrderSectionAsViewed(order);

            order.AdditionalServicesViewed.Should().BeTrue();
        }

        [Test]
        public static void ChangePrice_ChangeValues_ExpectedPropertiesUpdated()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithProvisioningType(ProvisioningType.OnDemand)
                .WithDeliveryDate(DateTime.UtcNow)
                .WithEstimationPeriod(TimeUnit.PerYear)
                .Build();

            var expected = new
            {
                DeliveryDate = orderItem.DeliveryDate?.AddDays(1),
                Quantity = orderItem.Quantity + 1,
                EstimationPeriod = TimeUnit.PerMonth,
                Price = orderItem.Price + 1m,
            };

            orderItem.ChangePrice(
                expected.DeliveryDate,
                expected.Quantity,
                expected.EstimationPeriod,
                expected.Price,
                null);

            orderItem.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static void UpdateFrom_ExpectedPropertiesUpdated()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithProvisioningType(ProvisioningType.OnDemand)
                .WithDeliveryDate(DateTime.UtcNow)
                .WithEstimationPeriod(TimeUnit.PerYear)
                .Build();

            var expected = new
            {
                DeliveryDate = orderItem.DeliveryDate?.AddDays(1),
                Quantity = orderItem.Quantity + 1,
                EstimationPeriod = TimeUnit.PerMonth,
                Price = orderItem.Price + 1.00m,
            };

            var updatedOrderItem = OrderItemBuilder
                .Create()
                .WithDeliveryDate(expected.DeliveryDate)
                .WithEstimationPeriod(expected.EstimationPeriod)
                .WithQuantity(expected.Quantity)
                .WithPrice(expected.Price)
                .Build();

            orderItem.UpdateFrom(updatedOrderItem);

            orderItem.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static void ChangePrice_ChangeValues_NullEstimationPeriod_ExpectedPropertiesUpdated()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithProvisioningType(ProvisioningType.Patient)
                .WithDeliveryDate(DateTime.UtcNow)
                .WithEstimationPeriod(TimeUnit.PerYear)
                .Build();

            var expected = new
            {
                DeliveryDate = orderItem.DeliveryDate?.AddDays(1),
                Quantity = orderItem.Quantity + 1,
                EstimationPeriod = TimeUnit.PerYear,
                Price = orderItem.Price + 1m,
            };

            orderItem.ChangePrice(
                expected.DeliveryDate,
                expected.Quantity,
                null,
                expected.Price,
                null);

            orderItem.Should().BeEquivalentTo(expected);
        }

        [TestCaseSource(nameof(ChangePriceTestCases))]
        public static void ChangePrice_SetsExpectedUpdatedValue(
            OrderItem orderItem,
            DateTime deliveryDate,
            TimeUnit estimationPeriod,
            int quantity,
            decimal price,
            bool expectedValue)
        {
            orderItem.Updated.Should().BeFalse();

            orderItem.ChangePrice(deliveryDate, quantity, estimationPeriod, price, null);

            orderItem.Updated.Should().Be(expectedValue);
        }

        [TestCase(0, 0, false, 0, 0)]
        [TestCase(1, 0, false, 0, 1)]
        [TestCase(0, 1, false, 0, 1)]
        [TestCase(0, 0, true, 0, 1)]
        [TestCase(0, 0, false, 1, 1)]
        public static void ChangePrice_Callback_ReturnsExpectedCallbackCount(
            int deliveryDateChangeInput,
            int quantityChangeInput,
            bool estimationPeriodChangeInput,
            decimal priceChangeInput,
            int expectedCount)
        {
            int callbackCounter = 0;

            var orderItem = OrderItemBuilder
                .Create()
                .WithProvisioningType(ProvisioningType.OnDemand)
                .WithEstimationPeriod(TimeUnit.PerYear)
                .Build();

            orderItem.ChangePrice(
                orderItem.DeliveryDate?.AddDays(deliveryDateChangeInput),
                orderItem.Quantity + quantityChangeInput,
                estimationPeriodChangeInput ? TimeUnit.PerMonth : TimeUnit.PerYear,
                orderItem.Price + priceChangeInput,
                () => callbackCounter++);

            callbackCounter.Should().Be(expectedCount);
        }

        [Test]
        public static void Equals_DifferentType_ReturnsFalse()
        {
            var item = OrderItemBuilder.Create().Build();
            var anonItem = new
            {
                item.OdsCode,
                item.CatalogueItemId,
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
        public static void Equals_TwoNewOrderItem_AreNotEqual()
        {
            var actual = OrderItemBuilder
                .Create()
                .Build();

            var expected = OrderItemBuilder
                .Create()
                .Build();

            actual.Equals(expected).Should().BeFalse();
        }

        [TestCase(123, 456, false)]
        [TestCase(123, 123, true)]
        public static void Equals_DifferentOrderItemId_AreNotEqual(
            int firstOrderItemIdInput,
            int secondOrderItemIdInput,
            bool expected)
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(firstOrderItemIdInput)
                .Build();

            var comparisonObject = OrderItemBuilder
                .Create()
                .WithOrderItemId(secondOrderItemIdInput)
                .Build();

            orderItem.Equals(comparisonObject).Should().Be(expected);
        }

        [Test]
        public static void GetHashCode_MatchOrderItemId()
        {
            const int expected = 123;

            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(expected)
                .Build();

            orderItem.GetHashCode().Should().Be(expected);
        }

        [Test]
        public static void GetHashCode_TwoOrderItems_HashCodeNotEqual()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .Build();

            var orderItemComparison = OrderItemBuilder
                .Create()
                .Build();

            var actual = orderItem.GetHashCode();
            var expected = orderItemComparison.GetHashCode();

            actual.Should().NotBe(expected);
        }

        [Test]
        public static void CalculateCost_WithPriceTypePerMonth_CalculatesCostCorrectly()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithPrice(1m)
                .WithQuantity(10)
                .WithPriceTimeUnit(TimeUnit.PerMonth)
                .Build();

            orderItem.CalculateTotalCostPerYear().Should().Be(120);
        }

        [Test]
        public static void CalculateCost_WithPriceTypePerYear_CalculatesCostCorrectly()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithPrice(1m)
                .WithQuantity(10)
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
                .WithQuantity(10)
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
                .WithQuantity(10)
                .WithPriceTimeUnit(null)
                .WithEstimationPeriod(TimeUnit.PerYear)
                .Build();

            orderItem.CalculateTotalCostPerYear().Should().Be(1);
        }

        [Test]
        public static void CalculateCost_WithNoPriceTypeOrEstimationPeriod_PriceIsPriceTimesQuantity()
        {
            const int price = 26;
            const int quantity = 13;

            var orderItem = OrderItemBuilder
                .Create()
                .WithPrice(price)
                .WithQuantity(quantity)
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

        private static IEnumerable<ITestCaseData> ChangePriceTestCases()
        {
            const TimeUnit estimationPeriod = TimeUnit.PerMonth;
            const int quantity = 1;
            const decimal price = 1.00m;

            var deliveryDate = DateTime.UtcNow;

            OrderItem OrderItem() =>
                OrderItemBuilder.Create()
                    .WithDeliveryDate(deliveryDate)
                    .WithEstimationPeriod(estimationPeriod)
                    .WithPrice(price)
                    .WithQuantity(quantity)
                    .Build();

            yield return new TestCaseData(OrderItem(), deliveryDate, estimationPeriod, quantity, price, false);
            yield return new TestCaseData(OrderItem(), deliveryDate.AddDays(1), estimationPeriod, quantity, price, true);
            yield return new TestCaseData(OrderItem(), deliveryDate, TimeUnit.PerYear, quantity, price, true);
            yield return new TestCaseData(OrderItem(), deliveryDate, estimationPeriod, quantity + 1, price, true);
            yield return new TestCaseData(OrderItem(), deliveryDate, estimationPeriod, quantity, price + 1.00m, true);
        }

        private static OrderItem CreateOrderItem(CatalogueItemType catalogueItemType, ProvisioningType provisioningType)
        {
            return OrderItemBuilder
                .Create()
                .WithCatalogueItemType(catalogueItemType)
                .WithProvisioningType(provisioningType)
                .Build();
        }
    }
}
