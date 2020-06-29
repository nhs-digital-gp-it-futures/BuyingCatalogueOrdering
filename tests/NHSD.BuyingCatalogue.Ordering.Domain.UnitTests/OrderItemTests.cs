using System;
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
        public void Constructor_NullOrEmptyOdsCode_ThrowsArgumentException(string odsCodeInput)
        {
            void Test()
            {
                OrderItemBuilder
                    .Create()
                    .WithOdsCode(odsCodeInput)
                    .Build();
            }

            Assert.Throws<ArgumentException>(Test);
        }

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
    }
}
