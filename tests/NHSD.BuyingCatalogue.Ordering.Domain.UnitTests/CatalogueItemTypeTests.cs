using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CatalogueItemTypeTests
    {
        [Test]
        public void List_ReturnsExpectedList()
        {
            var actual = CatalogueItemType.List();

            var expected = new List<CatalogueItemType>
            {
                CatalogueItemType.Solution, 
                CatalogueItemType.AdditionalService,
                CatalogueItemType.AssociatedService
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void FromId_CatalogueItemTypeId_ReturnsExpectedType()
        {
            var actual = CatalogueItemType.FromId(1);

            actual.Should().Be(CatalogueItemType.Solution);
        }

        [Test]
        public void FromId_UnknownCatalogueItemTypeId_ReturnsNull()
        {
            var actual = CatalogueItemType.FromId(10);
            actual.Should().BeNull();
        }

        [TestCase(null)]
        [TestCase("InvalidType")]
        public void Equals_ComparisonObject_AreNotEqual(object comparisonObject)
        {
            CatalogueItemType.Solution.Equals(comparisonObject).Should().BeFalse();
        }

        [Test]
        public void Equals_Same_AreEqual()
        {
            var instance = CatalogueItemType.Solution;
            instance.Equals(instance).Should().BeTrue();
        }

        [Test]
        public void Equals_Different_AreNotEqual()
        {
            var instance = CatalogueItemType.Solution;
            var comparisonObject = CatalogueItemType.AdditionalService;

            instance.Equals(comparisonObject).Should().BeFalse();
        }
    }
}
