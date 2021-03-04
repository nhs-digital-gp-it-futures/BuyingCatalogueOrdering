using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CatalogueItemTests
    {
        [Test]
        [CommonAutoData]
        public static void Equals_CatalogueItem_OtherIsNull_ReturnsFalse(CatalogueItem item)
        {
            var result = item.Equals(null);

            result.Should().BeFalse();
        }

        [Test]
        [CommonAutoData]
        public static void Equals_CatalogueItem_OtherIsThis_ReturnsTrue(CatalogueItem item)
        {
            var result = item.Equals(item);

            result.Should().BeTrue();
        }

        [Test]
        [CommonAutoData]
        public static void Equals_CatalogueItem_OtherHasSameId_ReturnsTrue(CatalogueItemId id)
        {
            var item1 = new CatalogueItem { Id = id };
            var item2 = new CatalogueItem { Id = id };

            var result = item1.Equals(item2);

            result.Should().BeTrue();
        }

        [Test]
        [CommonAutoData]
        public static void Equals_CatalogueItem_OtherHasDifferentId_ReturnsFalse(CatalogueItemId id1, CatalogueItemId id2)
        {
            var item1 = new CatalogueItem { Id = id1 };
            var item2 = new CatalogueItem { Id = id2 };

            var result = item1.Equals(item2);

            result.Should().BeFalse();
        }

        [Test]
        [CommonAutoData]
        public static void Equals_Object_OtherHasSameId_ReturnsTrue(CatalogueItemId id)
        {
            var item1 = new CatalogueItem { Id = id };
            object item2 = new CatalogueItem { Id = id };

            var result = item1.Equals(item2);

            result.Should().BeTrue();
        }

        [Test]
        [CommonAutoData]
        public static void Equals_Object_OtherIsDifferentType_ReturnsFalse(CatalogueItemId id)
        {
            var item1 = new CatalogueItem { Id = id };
            var item2 = new { Id = id };

            var result = item1.Equals(item2);

            result.Should().BeFalse();
        }

        [Test]
        [CommonAutoData]
        public static void GetHashCode_ReturnsExpectedResult(CatalogueItemId id)
        {
            var item = new CatalogueItem { Id = id };

            var hash = item.GetHashCode();

            hash.Should().Be(item.Id.GetHashCode());
        }
    }
}
