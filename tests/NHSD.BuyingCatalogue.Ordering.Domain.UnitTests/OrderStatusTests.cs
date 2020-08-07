using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrderStatusTests
    {
        [Test]
        public void List_ReturnsExpectedList()
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
        public void FromId_OrderStatusId_ReturnsExpectedType()
        {
            var actual = OrderStatus.FromId(1);

            actual.Should().Be(OrderStatus.Complete);
        }

        [Test]
        public void FromId_UnknownOrderStatusId_ReturnsNull()
        {
            var actual = OrderStatus.FromId(10);
            actual.Should().BeNull();
        }

        [TestCase("Complete")]
        [TestCase("complete")]
        [TestCase("comPLete")]
        public void FromName_OrderStatusName_ReturnsExpectedType(string orderStatusName)
        {
            var actual = OrderStatus.FromName(orderStatusName);

            actual.Should().Be(OrderStatus.Complete);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("Unknown")]
        public void FromName_InvalidOrderStatusName_ReturnsNull(string orderStatusName)
        {
            var actual = OrderStatus.FromName(orderStatusName);
            actual.Should().BeNull();
        }

        [TestCase(null)]
        [TestCase("InvalidType")]
        public void Equals_ComparisonObject_AreNotEqual(object comparisonObject)
        {
            OrderStatus.Incomplete.Equals(comparisonObject).Should().BeFalse();
        }

        [Test]
        public void Equals_Same_AreEqual()
        {
            var instance = OrderStatus.Incomplete;
            instance.Equals(instance).Should().BeTrue();
        }

        [Test]
        public void Equals_Different_AreNotEqual()
        {
            var instance = OrderStatus.Incomplete;
            var comparisonObject = OrderStatus.Complete;

            instance.Equals(comparisonObject).Should().BeFalse();
        }
    }
}
