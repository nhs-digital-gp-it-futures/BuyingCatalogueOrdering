using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    internal sealed class TimeUnitExtensionsTests
    {
        [Test]
        public void ToModel_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => TimeUnitExtensions.ToModel(null));
        }

        [Test]
        public void ToModel_Month_ReturnsExpected()
        {
            TimeUnit sut = TimeUnit.PerMonth;

            var actual = sut.ToModel();
            actual.Should().BeEquivalentTo(new TimeUnitModel
            {
                Name = sut.Name,
                Description = sut.Description
            });
        }
    }
}
