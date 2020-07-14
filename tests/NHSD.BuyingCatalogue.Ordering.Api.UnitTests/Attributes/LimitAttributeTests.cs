using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Attributes
{
    [TestFixture]
    public sealed class LimitAttributeTests
    {
        [TestCase(1, LimitType.Maximum, 2, false)]
        [TestCase(1, LimitType.Maximum, 1, true)]
        [TestCase(1, LimitType.Maximum, int.MinValue, true)]
        [TestCase(1, LimitType.Minimum, 0, false)]
        [TestCase(1, LimitType.Minimum, 1, true)]
        [TestCase(1, LimitType.Minimum, int.MaxValue, true)]
        public void IsValid_Int_ReturnsCorrectValue(int value, LimitType limitType, int input, bool expected)
        {
            var attribute = new LimitAttribute(value, limitType);
            attribute.IsValid(input).Should().Be(expected);
        }

        [TestCase(1, LimitType.Maximum, 1.0000001d, false)]
        [TestCase(1, LimitType.Maximum, 1d, true)]
        [TestCase(1, LimitType.Maximum, double.MinValue, true)]
        [TestCase(1, LimitType.Minimum, 0.9999999d, false)]
        [TestCase(1, LimitType.Minimum, 1d, true)]
        [TestCase(1, LimitType.Minimum, double.MaxValue, true)]
        public void IsValid_Double_ReturnsCorrectValue(double value, LimitType limitType, double input, bool expected)
        {
            var attribute = new LimitAttribute(value, limitType);
            attribute.IsValid(input).Should().Be(expected);
        }

        [TestCase("1", LimitType.Maximum, "1.0000001", false)]
        [TestCase("1", LimitType.Maximum, "1", true)]
        [TestCase("1", LimitType.Maximum, "-99999999999.999", true)]
        [TestCase("1", LimitType.Minimum, "0.9999999", false)]
        [TestCase("1", LimitType.Minimum, "1", true)]
        [TestCase("1", LimitType.Minimum, "9999999999999.999", true)]
        public void IsValid_Decimal_ReturnsCorrectValue(string value, LimitType limitType, string input, bool expected)
        {
            var convertedInput = Convert.ToDecimal(input);
            var attribute = new LimitAttribute(typeof(decimal), value, limitType);
            attribute.IsValid(convertedInput).Should().Be(expected);
        }

        [Test]
        public void IsValid_NullInput_ReturnsTrue()
        {
            var attribute = new LimitAttribute(1, LimitType.Minimum);
            attribute.IsValid(null).Should().BeTrue();
        }

        [Test]
        public void IsValid_EmptyString_ReturnsTrue()
        {
            var attribute = new LimitAttribute(1, LimitType.Minimum);
            attribute.IsValid(string.Empty);
        }

        [Test]
        public void IsValid_InvalidStringAsInteger_ReturnsFalse()
        {
            var attribute = new LimitAttribute(1, LimitType.Minimum);
            attribute.IsValid("Moose").Should().BeFalse();
        }

        [Test]
        public void IsValid_InvalidStringAsDouble_ReturnsFalse()
        {
            var attribute = new LimitAttribute(1d, LimitType.Minimum);
            attribute.IsValid("Moose").Should().BeFalse();
        }

        [Test]
        public void IsValid_NonIComparableType_ThrowsException()
        {
            var attribute = new LimitAttribute(typeof(LimitAttribute), "I will fail", LimitType.Minimum);
            Assert.Throws<InvalidOperationException>(() => attribute.IsValid(1));
        }

        [Test]
        public void IsValid_NullOperandType_ThrowsException()
        {
            var attribute = new LimitAttribute(null, "I will fail", LimitType.Minimum);
            Assert.Throws<InvalidOperationException>(() => attribute.IsValid(1));
        }
    }
}
