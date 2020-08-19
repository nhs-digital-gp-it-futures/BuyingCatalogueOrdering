using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Requests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CompleteOrderRequestTests
    {
        [Test]
        public void Constructor_NullOrder_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CompleteOrderRequest(null));
        }

        [Test]
        public void Order_GetOrderProperty_ReturnsExpectedOrder()
        {
            var expectedOrder = OrderBuilder.Create().Build();

            var actual = new CompleteOrderRequest(expectedOrder).Order;

            actual.Should().Be(expectedOrder);
        }
    }
}
