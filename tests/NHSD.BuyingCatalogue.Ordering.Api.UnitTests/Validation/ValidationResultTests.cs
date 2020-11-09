using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Validation
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class ValidationResultTests
    {
        [Test]
        [OrderingAutoData]
        public static void Constructor_InitializesErrors(
            [Frozen] IReadOnlyList<ErrorDetails> errors,
            ValidationResult result)
        {
            result.Errors.Should().BeSameAs(errors);
        }

        [Test]
        [OrderingAutoData]
        public static void Success_Errors_IsFalse(ValidationResult result)
        {
            result.Success.Should().BeFalse();
        }

        [Test]
        public static void Success_NoErrors_IsTrue()
        {
            var result = new ValidationResult(Array.Empty<ErrorDetails>());

            result.Success.Should().BeTrue();
        }
    }
}
