using System;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
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
    }
}
