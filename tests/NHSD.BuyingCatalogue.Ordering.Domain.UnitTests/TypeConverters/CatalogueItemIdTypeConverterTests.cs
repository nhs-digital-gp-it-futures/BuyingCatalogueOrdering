﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain.TypeConverters;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests.TypeConverters
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CatalogueItemIdTypeConverterTests
    {
        [Test]
        [CommonInlineAutoData(typeof(string), true)]
        [CommonInlineAutoData(typeof(int), false)]
        public static void CanConvertFrom_ReturnsExpectedResult(
            Type sourceType,
            bool expectedResult,
            ITypeDescriptorContext context,
            CatalogueItemIdTypeConverter typeConverter)
        {
            var actualResult = typeConverter.CanConvertFrom(context, sourceType);

            actualResult.Should().Be(expectedResult);
        }

        [Test]
        [CommonAutoData]
        public static void ConvertFrom_InvalidId_ReturnsExpectedResult(
            ITypeDescriptorContext context,
            CatalogueItemIdTypeConverter typeConverter)
        {
            Assert.Throws<FormatException>(
                () => typeConverter.ConvertFrom(context, CultureInfo.InvariantCulture, "NotCatalogueItemId"));
        }

        [Test]
        [CommonAutoData]
        public static void ConvertFrom_ValidId_ReturnsExpectedResult(
            ITypeDescriptorContext context,
            CatalogueItemIdTypeConverter typeConverter)
        {
            var actualResult = typeConverter.ConvertFrom(context, CultureInfo.InvariantCulture, "1000-001");

            actualResult.Should().Be(new CatalogueItemId(1000, "001"));
        }
    }
}
