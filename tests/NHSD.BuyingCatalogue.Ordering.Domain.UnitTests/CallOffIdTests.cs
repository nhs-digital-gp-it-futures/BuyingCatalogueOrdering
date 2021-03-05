using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CallOffIdTests
    {
        [TestCase(-1)]
        [TestCase(CallOffId.MaxId + 1)]
        public static void Constructor_Int_Int_IdOutOfRange_ThrowsException(int id)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new CallOffId(id, 1));
        }

        [TestCase(CallOffId.MaxRevision + 1)]
        public static void Constructor_Int_Int_RevisionOutOfRange_ThrowsException(byte revision)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new CallOffId(1, revision));
        }

        [TestCase("100000-01")]
        [TestCase("C10000001")]
        [TestCase("C100000-A")]
        [TestCase("C10000A-01")]
        [TestCase("BC100000-01")]
        [TestCase("C100000-001")]
        [TestCase("C0100000-01")]
        public static void Parse_InvalidId_ReturnsExpectedResult(string id)
        {
            (bool success, _) = CallOffId.Parse(id);

            success.Should().BeFalse();
        }

        [Test]
        public static void Parse_ValidId_ReturnsExpectedResult()
        {
            (bool success, CallOffId id) = CallOffId.Parse("C999999-99");

            success.Should().BeTrue();
            id.Should().Be(new CallOffId(999999, 99));
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        public static void Equals_CallOffId_ReturnsExpectedResult(int id1, int id2, bool expectedResult)
        {
            var callOffId1 = new CallOffId(id1, 1);
            var callOffId2 = new CallOffId(id2, 1);

            var result = callOffId1.Equals(callOffId2);

            result.Should().Be(expectedResult);
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        public static void Equals_Object_ReturnsExpectedResult(int id1, int id2, bool expectedResult)
        {
            var callOffId1 = new CallOffId(id1, 1);
            object callOffId2 = new CallOffId(id2, 1);

            var result = callOffId1.Equals(callOffId2);

            result.Should().Be(expectedResult);
        }

        [Test]
        public static void Equals_Object_DifferentType_ReturnsFalse()
        {
            var callOffId = new CallOffId(1, 1);
            var obj = new { Id = 1, Revision = 1 };

            var result = callOffId.Equals(obj);

            result.Should().BeFalse();
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        public static void Equals_Operator_ReturnsExpectedResult(int id1, int id2, bool expectedResult)
        {
            var callOffId1 = new CallOffId(id1, 1);
            var callOffId2 = new CallOffId(id2, 1);

            var result = callOffId1 == callOffId2;

            result.Should().Be(expectedResult);
        }

        [TestCase(1, 1, false)]
        [TestCase(1, 2, true)]
        public static void NotEquals_Operator_ReturnsExpectedResult(int id1, int id2, bool expectedResult)
        {
            var callOffId1 = new CallOffId(id1, 1);
            var callOffId2 = new CallOffId(id2, 1);

            var result = callOffId1 != callOffId2;

            result.Should().Be(expectedResult);
        }

        [Test]
        [CommonAutoData]
        public static void GetHashCode_ReturnsExpectedResult(CallOffId id)
        {
            id.GetHashCode().Should().Be(id.Id.GetHashCode());
        }

        [Test]
        public static void ToString_ReturnsExpectedResult()
        {
            var id = new CallOffId(1, 1);

            var idAsString = id.ToString();

            idAsString.Should().Be("C000001-01");
        }
    }
}
