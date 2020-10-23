using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class TimeUnitTests
    {
        [TestCase(TimeUnit.PerMonth, 12)]
        [TestCase(TimeUnit.PerYear, 1)]
        public static void EachTimeUnit_HasExpectedAmountInYear(TimeUnit timeUnit, int expectedAmountInYear)
        {
            var actualAmountInYear = timeUnit.AmountInYear();

            actualAmountInYear.Should().Be(expectedAmountInYear);
        }

        [TestCase(TimeUnit.PerMonth, "per month")]
        [TestCase(TimeUnit.PerYear, "per year")]
        public static void EachTimeUnit_HasExpectedDescription(TimeUnit timeUnit, string expectedDescription)
        {
            var actualDescription = timeUnit.Description();

            actualDescription.Should().Be(expectedDescription);
        }

        [TestCase(TimeUnit.PerMonth, "month")]
        [TestCase(TimeUnit.PerYear, "year")]
        public static void EachTimeUnit_HasExpectedDisplayName(TimeUnit timeUnit, string expectedDisplayName)
        {
            var actualDisplayName = timeUnit.Name();

            actualDisplayName.Should().Be(expectedDisplayName);
        }
    }
}
