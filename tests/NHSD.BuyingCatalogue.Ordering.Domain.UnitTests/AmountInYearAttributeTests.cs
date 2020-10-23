using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using Castle.Core.Internal;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class AmountInYearAttributeTests
    {
        [Test]
        [AutoData]
        public static void Constructor_InitializesAmountInYear(int amount)
        {
            var attribute = new AmountInYearAttribute(amount);

            attribute.AmountInYear.Should().Be(amount);
        }

        [Test]
        public static void DoesNotAllowMultiple()
        {
            var usage = typeof(AmountInYearAttribute).GetAttributeUsage();

            usage.AllowMultiple.Should().BeFalse();
        }

        [Test]
        public static void Targets_Fields()
        {
            var usage = typeof(AmountInYearAttribute).GetAttributeUsage();

            usage.ValidOn.Should().Be(AttributeTargets.Field);
        }
    }
}
