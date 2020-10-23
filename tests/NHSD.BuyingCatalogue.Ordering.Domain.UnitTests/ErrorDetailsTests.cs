using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class ErrorDetailsTests
    {
        [Test]
        public static void Constructor_String_String_NullId_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new ErrorDetails(null, "field"));
        }

        [Test]
        [AutoData]
        public static void Constructor_String_InitializesId(string id)
        {
            var details = new ErrorDetails(id);

            details.Id.Should().Be(id);
        }

        [Test]
        [AutoData]
        public static void Constructor_String_InitializesFieldAsNull(string id)
        {
            var details = new ErrorDetails(id);

            details.Field.Should().BeNull();
        }

        [Test]
        [AutoData]
        public static void Constructor_String_String_InitializesId(string id)
        {
            var details = new ErrorDetails(id, null);

            details.Id.Should().Be(id);
        }

        [Test]
        [AutoData]
        public static void Constructor_String_String_InitializesField(string id, string field)
        {
            var details = new ErrorDetails(id, field);

            details.Field.Should().Be(field);
        }

        [Test]
        [AutoData]
        public static void Equals_DifferentType_ReturnsFalse(ErrorDetails details)
        {
            var anonErrorDetails = new
            {
                Id = "Id",
                Field = "Field",
            };

            var isEqual = details.Equals(anonErrorDetails);

            isEqual.Should().BeFalse();
        }

        [TestCaseSource(nameof(EqualityTestCases))]
        public static void Equals_ReturnsExpectedResult(ErrorDetails a, ErrorDetails b, bool expectedResult)
        {
            var isEqual = a.Equals(b);

            isEqual.Should().Be(expectedResult);
        }

        [TestCaseSource(nameof(EqualityTestCases))]
        public static void GetHashCode_ReturnsExpectedResult(ErrorDetails a, ErrorDetails b, bool expectedResult)
        {
            var isEqual = a.GetHashCode() == b.GetHashCode();

            isEqual.Should().Be(expectedResult);
        }

        private static IEnumerable<ITestCaseData> EqualityTestCases()
        {
            const string id = "Id";
            const string field = "Field";

            var a = new ErrorDetails(id, field);

            yield return new TestCaseData(a, a, true);
            yield return new TestCaseData(a, new ErrorDetails(id, field), true);
            yield return new TestCaseData(a, new ErrorDetails("id", field), false);
            yield return new TestCaseData(a, new ErrorDetails(id, "field"), false);
        }
    }
}
