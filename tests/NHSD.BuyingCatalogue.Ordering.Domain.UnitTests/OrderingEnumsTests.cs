using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using EnumsNET;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderingEnumsTests
    {
        [Test]
        public static void ParseTimeUnit_Undefined_ReturnsZero()
        {
            var timeUnit = OrderingEnums.ParseTimeUnit("DoesNotExist");

            timeUnit.Should().Be(OrderingEnums.UndefinedTimeUnit);
        }

        [TestCase("month", TimeUnit.PerMonth)]
        [TestCase("perMonth", TimeUnit.PerMonth)]
        [TestCase("year", TimeUnit.PerYear)]
        [TestCase("perYear", TimeUnit.PerYear)]
        public static void ParseTimeUnit_ReturnsExpectedValue(string value, TimeUnit expectedResult)
        {
            var timeUnit = OrderingEnums.ParseTimeUnit(value);

            timeUnit.Should().Be(expectedResult);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        public static void Parse_NullOrWhiteSpaceValue_ReturnsNull(string value)
        {
            var enumValue = OrderingEnums.ParseTimeUnit(value);

            enumValue.Should().BeNull();
        }

        [Test]
        [AutoData]
        public static void Parse_ReturnsExpectedEnumValue(ProvisioningType provisioningType)
        {
            var enumValue = OrderingEnums.Parse<ProvisioningType>(provisioningType.GetName());

            enumValue.Should().Be(provisioningType);
        }

        [Test]
        public static void Parse_Undefined_ReturnsNull()
        {
            var enumValue = OrderingEnums.Parse<ProvisioningType>("DoesNotExist");

            enumValue.Should().BeNull();
        }

        [TestCase("flat", CataloguePriceType.Flat)]
        [TestCase("TIERED", CataloguePriceType.Tiered)]
        public static void ParseIgnoreCase_ReturnsExpectedEnumValue(string value, CataloguePriceType expected)
        {
            var actual = OrderingEnums.ParseStrictIgnoreCase<CataloguePriceType>(value);

            actual.Should().Be(expected);
        }

        [Test]
        public static void ParseIgnoreCase_InvalidValue_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => OrderingEnums.ParseStrictIgnoreCase<CataloguePriceType>("InvalidPriceType"));
        }
    }
}
