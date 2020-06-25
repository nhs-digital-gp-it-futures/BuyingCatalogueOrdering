using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Domain.UnitTests.Builders;
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
    }
}
