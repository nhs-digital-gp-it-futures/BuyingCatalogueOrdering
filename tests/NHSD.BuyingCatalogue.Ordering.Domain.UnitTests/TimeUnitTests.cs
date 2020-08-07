using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class TimeUnitTests
    {
        [Test]
        public void List_ReturnsExpectedList()
        {
            var actual = TimeUnit.List();

            var expected = new List<TimeUnit>
            {
                TimeUnit.PerYear, 
                TimeUnit.PerMonth
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void FromId_TimeUnitId_ReturnsExpectedType()
        {
            var actual = TimeUnit.FromId(1);

            actual.Should().Be(TimeUnit.PerMonth);
        }

        [Test]
        public void FromId_UnknownTimeUnitId_ReturnsNull()
        {
            var actual = TimeUnit.FromId(10);
            actual.Should().BeNull();
        }

        [TestCase("Month")]
        [TestCase("month")]
        [TestCase("mONTh")]
        public void FromName_TimeUnitName_ReturnsExpectedType(string timeUnitName)
        {
            var actual = TimeUnit.FromName(timeUnitName);

            actual.Should().Be(TimeUnit.PerMonth);
        }

        [TestCase("Unknown")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void FromName_UnknownTimeUnitName_ReturnsNull(string name)
        {
            var actual = TimeUnit.FromName(name);
            actual.Should().BeNull();
        }

        [TestCase(null)]
        [TestCase("InvalidType")]
        public void Equals_ComparisonObject_AreNotEqual(object comparisonObject)
        {
            TimeUnit.PerYear.Equals(comparisonObject).Should().BeFalse();
        }

        [Test]
        public void Equals_Same_AreEqual()
        {
            var instance = TimeUnit.PerYear;
            instance.Equals(instance).Should().BeTrue();
        }

        [Test]
        public void Equals_Different_AreNotEqual()
        {
            var instance = TimeUnit.PerYear;
            var comparisonObject = TimeUnit.PerMonth;

            instance.Equals(comparisonObject).Should().BeFalse();
        }
    }
}
