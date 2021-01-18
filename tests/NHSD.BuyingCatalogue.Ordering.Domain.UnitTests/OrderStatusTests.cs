using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrderStatusTests
    {
        [Test]
        public static void List_ReturnsExpectedList()
        {
            var actual = OrderStatus.List();

            var expected = new List<OrderStatus>
            {
                OrderStatus.Incomplete,
                OrderStatus.Complete,
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static void FromId_OrderStatusId_ReturnsExpectedType()
        {
            var actual = OrderStatus.FromId(1);

            actual.Should().Be(OrderStatus.Complete);
        }

        [Test]
        public static void FromId_UnknownOrderStatusId_ReturnsNull()
        {
            var actual = OrderStatus.FromId(10);
            actual.Should().BeNull();
        }

        [TestCase("Complete")]
        [TestCase("complete")]
        [TestCase("COMPLETE")]
        public static void FromName_OrderStatusName_ReturnsExpectedType(string orderStatusName)
        {
            var actual = OrderStatus.FromName(orderStatusName);

            actual.Should().Be(OrderStatus.Complete);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("Unknown")]
        public static void FromName_InvalidOrderStatusName_ReturnsNull(string orderStatusName)
        {
            var actual = OrderStatus.FromName(orderStatusName);
            actual.Should().BeNull();
        }

        [TestCase(null)]
        [TestCase("InvalidType")]
        public static void Equals_ComparisonObject_AreNotEqual(object comparisonObject)
        {
            OrderStatus.Incomplete.Equals(comparisonObject).Should().BeFalse();
        }

        [Test]
        public static void Equals_Same_AreEqual()
        {
            var instance = OrderStatus.Incomplete;
            instance.Equals(instance).Should().BeTrue();
        }

        [Test]
        public static void Equals_Different_AreNotEqual()
        {
            var instance = OrderStatus.Incomplete;
            var comparisonObject = OrderStatus.Complete;

            instance.Equals(comparisonObject).Should().BeFalse();
        }
    }
}
