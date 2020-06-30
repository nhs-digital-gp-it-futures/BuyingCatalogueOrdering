using System;
using System.Threading;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Domain.UnitTests.Builders;
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
        public void ChangePrice_ChangeValues_ExpectedPropertiesUpdated()
        {
            var orderItem = OrderItemBuilder
                .Create()
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
    }
}
