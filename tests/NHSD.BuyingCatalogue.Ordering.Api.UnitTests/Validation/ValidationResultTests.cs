using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            ValidationResult result)
        {
            result.Errors.Should().BeEmpty();
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_ErrorDetails_InitializesErrors(
            ErrorDetails error)
        {
            var result = new ValidationResult(error);

            result.Errors.Should().HaveCount(1);
            result.Errors.Should().Contain(error);
        }

        [Test]
        [OrderingAutoData]
        public static void Constructor_IReadOnlyList_ErrorDetails_InitializesErrors(
            IReadOnlyList<ErrorDetails> errors)
        {
            var result = new ValidationResult(errors);

            result.Errors.Should().BeSameAs(errors);
        }

        // ReSharper disable once IdentifierTypo
        [Test]
        [OrderingAutoData]
        public static void Constructor_Params_ValidationResult_InitializesErrors(
            IReadOnlyList<ErrorDetails> errors1,
            IReadOnlyList<ErrorDetails> errors2)
        {
            var result1 = new ValidationResult(errors1);
            var result2 = new ValidationResult(errors2);
            var result = new ValidationResult(result1, result2);

            result.Errors.Should().HaveCount(errors1.Count + errors2.Count);
            result.Errors.Should().Contain(errors1);
            result.Errors.Should().Contain(errors2);
        }

        [Test]
        [OrderingAutoData]
        public static void Success_Errors_IsFalse(IReadOnlyList<ErrorDetails> errors)
        {
            var result = new ValidationResult(errors);

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
