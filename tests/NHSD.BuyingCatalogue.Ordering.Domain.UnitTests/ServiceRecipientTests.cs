using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class ServiceRecipientTests
    {
        [Test]
        [AutoData]
        public static void Constructor_String_String_String_InitializesOrderId(string orderId)
        {
            var recipient = new ServiceRecipient(orderId, null, null);

            recipient.OrderId.Should().Be(orderId);
            recipient.Name.Should().BeNull();
            recipient.OdsCode.Should().BeNull();
        }

        [Test]
        [AutoData]
        public static void Constructor_String_String_String_InitializesOdsCode(string odsCode)
        {
            var recipient = new ServiceRecipient(null, odsCode, null);

            recipient.OdsCode.Should().Be(odsCode);
            recipient.Name.Should().BeNull();
            recipient.OrderId.Should().BeNull();
        }

        [Test]
        [AutoData]
        public static void Constructor_String_String_String_InitializesName(string name)
        {
            var recipient = new ServiceRecipient(null, null, name);

            recipient.Name.Should().Be(name);
            recipient.OrderId.Should().BeNull();
            recipient.OdsCode.Should().BeNull();
        }

        [Test]
        public static void Equals_DifferentType_ReturnsFalse()
        {
            var recipient = new ServiceRecipient("Id-1", "ODS", null);
            var anonRecipient = new
            {
                recipient.OrderId,
                recipient.OdsCode,
            };

            var isEqual = recipient.Equals(anonRecipient);

            isEqual.Should().BeFalse();
        }

        [TestCaseSource(nameof(EqualityTestCases))]
        public static void Equals_ReturnsExpectedResult(ServiceRecipient a, ServiceRecipient b, bool expectedResult)
        {
            var isEqual = a.Equals(b);

            isEqual.Should().Be(expectedResult);
        }

        [TestCaseSource(nameof(EqualityTestCases))]
        public static void GetHashCode_ReturnsExpectedResult(ServiceRecipient a, ServiceRecipient b, bool expectedResult)
        {
            var isEqual = a.GetHashCode() == b.GetHashCode();

            isEqual.Should().Be(expectedResult);
        }

        private static IEnumerable<ITestCaseData> EqualityTestCases()
        {
            const string orderId = "Id-1";
            const string odsCode = "ODS";

            var a = new ServiceRecipient(orderId, odsCode, null);

            yield return new TestCaseData(a, a, true);
            yield return new TestCaseData(a, new ServiceRecipient(orderId.ToUpperInvariant(), odsCode, null), true);
            yield return new TestCaseData(a, new ServiceRecipient("Id-2", odsCode, null), false);
            yield return new TestCaseData(a, new ServiceRecipient(orderId, "PDS", null), false);
        }
    }
}
