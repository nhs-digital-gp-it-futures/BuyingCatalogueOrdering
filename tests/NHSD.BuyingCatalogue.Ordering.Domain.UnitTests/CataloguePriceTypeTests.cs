using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CataloguePriceTypeTests
    {
        [Test]
        public void List_ReturnsExpectedList()
        {
            var actual = CataloguePriceType.List();

            var expected = new List<CataloguePriceType>
            {
                CataloguePriceType.Flat, 
                CataloguePriceType.Tiered,
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void FromId_CataloguePriceTypeId_ReturnsExpectedType()
        {
            var actual = CataloguePriceType.FromId(1);

            actual.Should().Be(CataloguePriceType.Flat);
        }

        [Test]
        public void FromId_UnknownCataloguePriceTypeId_ReturnsNull()
        {
            var actual = CataloguePriceType.FromId(10);
            actual.Should().BeNull();
        }

        [TestCase("Tiered")]
        [TestCase("tiered")]
        [TestCase("tiERed")]
        public void FromName_CataloguePriceTypeName_ReturnsExpectedType(string cataloguePriceTypeName)
        {
            var actual = CataloguePriceType.FromName(cataloguePriceTypeName);

            actual.Should().Be(CataloguePriceType.Tiered);
        }

        [Test]
        public void FromName_UnknownCataloguePriceTypeName_ReturnsNull()
        {
            var actual = CataloguePriceType.FromName("Unknown");
            actual.Should().BeNull();
        }

        [Test]
        public void FromName_NullName_ThrowsArgumentNullException()
        {
            static void Test()
            {
                CataloguePriceType.FromName(null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Equals_Null_AreNotEqual()
        {
            CataloguePriceType.Flat.Equals(null).Should().BeFalse();
        }

        [TestCase(null)]
        [TestCase("InvalidType")]
        public void Equals_ComparisonObject_AreNotEqual(object comparisonObject)
        {
            CataloguePriceType.Flat.Equals(comparisonObject).Should().BeFalse();
        }

        [Test]
        public void Equals_Same_AreEqual()
        {
            var instance = CataloguePriceType.Flat;
            instance.Equals(instance).Should().BeTrue();
        }

        [Test]
        public void Equals_Different_AreNotEqual()
        {
            var instance = CataloguePriceType.Flat;
            var comparisonObject = CataloguePriceType.Tiered;

            instance.Equals(comparisonObject).Should().BeFalse();
        }
    }
}
