using System;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CatalogueItemIdTests
    {
        [TestCase(0)]
        [TestCase(CatalogueItemId.MaxSupplierId + 1)]
        public static void Constructor_Int_String_SupplierIdOutOfRange_ThrowsException(int supplierId)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new CatalogueItemId(supplierId, "1"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        public static void Constructor_Int_String_ItemIdNullOrWhiteSpace_ThrowsException(string itemId)
        {
            Assert.Throws<ArgumentException>(() => _ = new CatalogueItemId(1, itemId));
        }

        [Test]
        public static void Constructor_Int_String_ItemIdOutOfRange_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => _ = new CatalogueItemId(1, new string('a', CatalogueItemId.MaxItemIdLength + 1)));
        }

        [TestCase("1000000-01")]
        [TestCase("A100000-01")]
        [TestCase("100000-12345678")]
        [TestCase("1000001234567")]
        [TestCase("-01")]
        [TestCase("C100000-")]
        [TestCase("-")]
        public static void Parse_InvalidId_ReturnsExpectedResult(string id)
        {
            (bool success, _) = CatalogueItemId.Parse(id);

            success.Should().BeFalse();
        }

        [Test]
        public static void Parse_ValidId_ReturnsExpectedResult()
        {
            (bool success, CatalogueItemId id) = CatalogueItemId.Parse("999999-ItemId");

            success.Should().BeTrue();
            id.Should().Be(new CatalogueItemId(999999, "ItemId"));
        }

        [TestCase("1000000-01")]
        [TestCase("A100000-01")]
        [TestCase("100000-12345678")]
        [TestCase("1000001234567")]
        [TestCase("-01")]
        [TestCase("C100000-")]
        [TestCase("-")]
        public static void ParseExact_InvalidId_ThrowsException(string id)
        {
            Assert.Throws<FormatException>(() => CatalogueItemId.ParseExact(id));
        }

        [Test]
        public static void ParseExact_ValidId_ReturnsExpectedResult()
        {
            var id = CatalogueItemId.ParseExact("999999-ItemId");

            id.Should().Be(new CatalogueItemId(999999, "ItemId"));
        }

        [TestCase(1, "1", 1, "1", true)]
        [TestCase(1, "1", 1, "2", false)]
        [TestCase(1, "1", 2, "1", false)]
        public static void Equals_CatalogueItemId_ReturnsExpectedResult(
            int supplierId1,
            string itemId1,
            int supplierId2,
            string itemId2,
            bool expectedResult)
        {
            var catalogueItemId1 = new CatalogueItemId(supplierId1, itemId1);
            var catalogueItemId2 = new CatalogueItemId(supplierId2, itemId2);

            var result = catalogueItemId1.Equals(catalogueItemId2);

            result.Should().Be(expectedResult);
        }

        [TestCase(1, "1", 1, "1", true)]
        [TestCase(1, "1", 1, "2", false)]
        [TestCase(1, "1", 2, "1", false)]
        public static void Equals_Object_ReturnsExpectedResult(
            int supplierId1,
            string itemId1,
            int supplierId2,
            string itemId2,
            bool expectedResult)
        {
            var catalogueItemId1 = new CatalogueItemId(supplierId1, itemId1);
            object catalogueItemId2 = new CatalogueItemId(supplierId2, itemId2);

            var result = catalogueItemId1.Equals(catalogueItemId2);

            result.Should().Be(expectedResult);
        }

        [Test]
        public static void Equals_Object_DifferentType_ReturnsFalse()
        {
            var catalogueItemId = new CatalogueItemId(1, "1");
            var obj = new { SupplierId = 1, ItemId = 1 };

            var result = catalogueItemId.Equals(obj);

            result.Should().BeFalse();
        }

        [TestCase(1, "1", 1, "1", true)]
        [TestCase(1, "1", 1, "2", false)]
        [TestCase(1, "1", 2, "1", false)]
        public static void Equals_Operator_ReturnsExpectedResult(
            int supplierId1,
            string itemId1,
            int supplierId2,
            string itemId2,
            bool expectedResult)
        {
            var catalogueItemId1 = new CatalogueItemId(supplierId1, itemId1);
            var catalogueItemId2 = new CatalogueItemId(supplierId2, itemId2);

            var result = catalogueItemId1 == catalogueItemId2;

            result.Should().Be(expectedResult);
        }

        [TestCase(1, "1", 1, "1", false)]
        [TestCase(1, "1", 1, "2", true)]
        [TestCase(1, "1", 2, "1", true)]
        public static void NotEquals_Operator_ReturnsExpectedResult(
            int supplierId1,
            string itemId1,
            int supplierId2,
            string itemId2,
            bool expectedResult)
        {
            var catalogueItemId1 = new CatalogueItemId(supplierId1, itemId1);
            var catalogueItemId2 = new CatalogueItemId(supplierId2, itemId2);

            var result = catalogueItemId1 != catalogueItemId2;

            result.Should().Be(expectedResult);
        }

        [TestCase(1, "1", 1, "1", true)]
        [TestCase(1, "1", 1, "2", false)]
        [TestCase(1, "1", 2, "1", false)]
        public static void GetHashCode_ReturnsExpectedResult(
            int supplierId1,
            string itemId1,
            int supplierId2,
            string itemId2,
            bool expectedResult)
        {
            var catalogueItemId1 = new CatalogueItemId(supplierId1, itemId1);
            var catalogueItemId2 = new CatalogueItemId(supplierId2, itemId2);

            var hash1 = catalogueItemId1.GetHashCode();
            var hash2 = catalogueItemId2.GetHashCode();

            (hash1 == hash2).Should().Be(expectedResult);
        }

        [Test]
        public static void ToString_ReturnsExpectedResult()
        {
            var id = new CatalogueItemId(1, "1");

            var idAsString = id.ToString();

            idAsString.Should().Be("1-1");
        }
    }
}
