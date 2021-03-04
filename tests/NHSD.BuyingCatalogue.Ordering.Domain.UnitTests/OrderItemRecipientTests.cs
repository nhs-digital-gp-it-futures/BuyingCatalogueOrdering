using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrderItemRecipientTests
    {
        [Test]
        [CommonInlineAutoData(null, 100)]
        [CommonInlineAutoData(TimeUnit.PerYear, 100)]
        [CommonInlineAutoData(TimeUnit.PerMonth, 1200)]
        public static void CalculateTotalCostPerYear_ReturnsExpectedValue(
            TimeUnit? unit,
            int expectedCost,
            OrderItemRecipient recipient)
        {
            recipient.Quantity = 10;

            var actualCost = recipient.CalculateTotalCostPerYear(10, unit);

            actualCost.Should().Be(expectedCost);
        }
    }
}
