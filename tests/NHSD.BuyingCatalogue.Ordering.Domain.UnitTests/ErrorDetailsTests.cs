using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class ErrorDetailsTests
    {
        [Test]
        public static void Constructor_String_String_String_NullId_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new ErrorDetails("parentName", "field", null));
        }

        [Test]
        [AutoData]
        public static void Constructor_String_String_String_InitializesParentName(string parentName, string id)
        {
            var details = new ErrorDetails(parentName, null, id);

            details.ParentName.Should().Be(parentName);
        }

        [Test]
        [AutoData]
        public static void Constructor_String_String_String_InitializesField(string field, string id)
        {
            var details = new ErrorDetails(null, field, id);

            details.Field.Should().Be(field);
        }

        [Test]
        [AutoData]
        public static void Constructor_String_String_String_InitializesId(string id)
        {
            var details = new ErrorDetails(null, null, id);

            details.Id.Should().Be(id);
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
        public static void Constructor_String_String_InitializesParentNameAsNull(string id)
        {
            var details = new ErrorDetails(id, null);

            details.ParentName.Should().BeEmpty();
        }

        [Test]
        [AutoData]
        public static void Constructor_String_InitializesFieldAsNull(string id)
        {
            var details = new ErrorDetails(id);

            details.Field.Should().BeNull();
        }
    }
}
