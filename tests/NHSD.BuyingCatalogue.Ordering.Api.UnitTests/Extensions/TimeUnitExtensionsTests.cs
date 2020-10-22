using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class TimeUnitExtensionsTests
    {
        [Test]
        public static void ToModel_Month_ReturnsExpected()
        {
            const TimeUnit unit = TimeUnit.PerMonth;

            var actual = Api.Extensions.TimeUnitExtensions.ToModel(unit);

            actual.Should().BeEquivalentTo(new TimeUnitModel
            {
                Name = "month",
                Description = "per month"
            });
        }
    }
}
