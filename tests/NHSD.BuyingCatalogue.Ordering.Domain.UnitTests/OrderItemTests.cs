using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrderItemTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Constructor_NullOrEmptyCatalogueItemId_ThrowsArgumentException(string catalogueItemIdInput)
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
        public void Constructor_NullOrEmptyCurrencyCode_ThrowsArgumentException(string currencyCodeInput)
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
        public void Constructor_NullOrEmptyCatalogueItemName_ThrowsArgumentException(string catalogueItemNameInput)
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
        public void Constructor_NullCatalogueItemType_ThrowsArgumentNullException()
        {
            static void Test()
            {
                OrderItemBuilder
                    .Create()
                    .WithCatalogueItemType(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Constructor_NullProvisioningType_ThrowsArgumentNullException()
        {
            static void Test()
            {
                OrderItemBuilder
                    .Create()
                    .WithProvisioningType(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Constructor_NullCataloguePriceType_ThrowsArgumentNullException()
        {
            static void Test()
            {
                OrderItemBuilder
                    .Create()
                    .WithCataloguePriceType(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Constructor_NullCataloguePriceUnit_ThrowsArgumentNullException()
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
        public void MarkOrderSectionAsViewed_NullOrder_ThrowsArgumentNullException()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .Build();

            Assert.Throws<ArgumentNullException>(() => orderItem.MarkOrderSectionAsViewed(null));
        }

        [Test]
        public void MarkOrderSectionAsViewed_Order_CatalogueSolutionsViewedIsTrue()
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
        public void MarkOrderSectionAsViewed_Order_AdditionalServicesViewedIsTrue()
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
        public void ChangePrice_ChangeValues_ExpectedPropertiesUpdated()
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
                Price = orderItem.Price + 1m
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
        public void ChangePrice_ChangeValues_NullEstimationPeriod_ExpectedPropertiesUpdated()
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
                Price = orderItem.Price + 1m
            };

            orderItem.ChangePrice(
                expected.DeliveryDate,
                expected.Quantity,
                null,
                expected.Price,
                null);

            orderItem.Should().BeEquivalentTo(expected);
        }

        [TestCase(0, 0, false, 0, 0)]
        [TestCase(1, 0, false, 0, 1)]
        [TestCase(0, 1, false, 0, 1)]
        [TestCase(0, 0, true, 0, 1)]
        [TestCase(0, 0, false, 1, 1)]
        public void ChangePrice_Callback_ReturnsExpectedCallbackCount(
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
        public void Equals_NullOrderItem_AreNotEqual()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .Build();

            orderItem.Equals(null).Should().BeFalse();
        }

        [Test]
        public void Equals_SameOrderItemReference_AreEqual()
        {
            var orderItem = OrderItemBuilder
                .Create()
                .Build();

            orderItem.Equals(orderItem).Should().BeTrue();
        }

        [Test]
        public void Equals_TwoNewOrderItem_AreNotEqual()
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
        public void Equals_DifferentOrderItemId_AreNotEqual(
            int firstOrderItemIdInput,
            int secondOrderItemIdInput,
            bool expected
            )
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
        public void GetHashCode_MatchOrderItemId()
        {
            const int expected = 123;

            var orderItem = OrderItemBuilder
                .Create()
                .WithOrderItemId(expected)
                .Build();

            orderItem.GetHashCode().Should().Be(expected);
        }

        [Test]
        public void GetHashCode_TwoOrderItems_HashCodeNotEqual()
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
        public void CalculateCost_WithPriceTypePerMonth_CalculatesCostCorrectly()
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
        public void CalculateCost_WithPriceTypePerYear_CalculatesCostCorrectly()
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
        public void CalculateTotalCostPerYear_WithEstimationPeriodPerMonth_CalculatesCorrectly()
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
        public void CalculateTotalCostPerYear_WithEstimationPeriodPerYear_CalculatesCorrectly()
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
        public void CalculateCost_WithNoPriceTypeOrEstimationPeriod_PriceIsPriceTimesQuanitiy()
        {
            var price = 26;
            var quantity = 13;
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
        public void CostType_OrderItemIsOneOff_ReturnsCorrectCostType()
        {
            var orderItem = CreateOrderItem(CatalogueItemType.AssociatedService, ProvisioningType.Declarative);
            orderItem.CostType.Should().Be(CostType.OneOff);
        }

        [TestCase(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Declarative), CostType.Recurring)]
        [TestCase(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.OnDemand), CostType.Recurring)]
        [TestCase(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Patient), CostType.Recurring)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Declarative), CostType.Recurring)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.OnDemand), CostType.Recurring)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Patient), CostType.Recurring)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), nameof(ProvisioningType.OnDemand), CostType.Recurring)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), nameof(ProvisioningType.Patient), CostType.Recurring)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), nameof(ProvisioningType.Declarative), CostType.OneOff)]
        public void CostType_DeterminesTheCostType_ReturnsCorrectCostType(string catalogueItemTypeName, string provisioningTypeName, CostType costType)
        {
            var catalogueItemType = CatalogueItemType.FromName(catalogueItemTypeName);
            var provisioningType = ProvisioningType.FromName(provisioningTypeName);
            var orderItem = CreateOrderItem(catalogueItemType, provisioningType);

            orderItem.CostType.Should().Be(costType);
        }

        private OrderItem CreateOrderItem(CatalogueItemType catalogueItemType, ProvisioningType provisioningType)
        {
            return OrderItemBuilder
                .Create()
                .WithCatalogueItemType(catalogueItemType)
                .WithProvisioningType(provisioningType)
                .Build();
        }
    }
}
