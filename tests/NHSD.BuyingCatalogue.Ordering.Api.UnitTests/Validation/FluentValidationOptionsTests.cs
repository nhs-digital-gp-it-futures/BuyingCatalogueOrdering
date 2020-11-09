using System.Reflection;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Validation
{
    [TestFixture]
    internal static class FluentValidationOptionsTests
    {
        internal static string TestProperty { get; } = "Test";

        [Test]
        public static void DisplayMember_ReturnsExpectedValue()
        {
            var propertyInfo = typeof(FluentValidationOptionsTests).GetProperty(
                nameof(TestProperty),
                BindingFlags.Static | BindingFlags.NonPublic);

            var displayName = FluentValidationOptions.DisplayNameResolver(null, propertyInfo, null);

            displayName.Should().Be(nameof(TestProperty));
        }

        [Test]
        public static void DisplayMember_NullMemberInfo_ReturnsNull()
        {
            var displayName = FluentValidationOptions.DisplayNameResolver(null, null, null);

            displayName.Should().BeNull();
        }
    }
}
