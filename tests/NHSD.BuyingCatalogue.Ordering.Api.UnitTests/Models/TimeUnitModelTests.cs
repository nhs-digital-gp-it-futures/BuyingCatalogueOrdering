using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class TimeUnitModelTests
    {
        [Test]
        [OrderingAutoData]
        public static void ToTimeUnit_ReturnsExpectedTimeUnit(
            [Frozen] TimeUnit expectedTimeUnit,
            TimeUnitModel model)
        {
            var actualTimeUnit = model.ToTimeUnit();

            actualTimeUnit.Should().Be(expectedTimeUnit);
        }
    }
}
