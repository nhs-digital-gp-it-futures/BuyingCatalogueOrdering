using System;
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
    internal static class AggregateValidationResultTests
    {
        [Test]
        [OrderingAutoData]
        public static void AddValidationResult_NullResult_ThrowsException(AggregateValidationResult aggregateResult)
        {
            Assert.Throws<ArgumentNullException>(() => aggregateResult.AddValidationResult(null, 0));
        }

        [Test]
        [OrderingAutoData]
        public static void AddValidationResult_CombinesExistingResults(
            ErrorDetails error1,
            ErrorDetails error2,
            AggregateValidationResult aggregateResult)
        {
            var result1 = new ValidationResult(error1);
            var result2 = new ValidationResult(error2);

            aggregateResult.AddValidationResult(result1, 0);
            aggregateResult.AddValidationResult(result2, 0);

            aggregateResult.FailedValidations.Should().HaveCount(1);
        }

        [Test]
        [OrderingAutoData]
        public static void Success_SuccessfulResultsOnly_IsTrue(
            ValidationResult result,
            AggregateValidationResult aggregateResult)
        {
            aggregateResult.AddValidationResult(result, 0);

            aggregateResult.Success.Should().BeTrue();
        }

        [Test]
        [OrderingAutoData]
        public static void Success_FailedResults_IsFalse(
            ErrorDetails error,
            AggregateValidationResult aggregateResult)
        {
            var result = new ValidationResult(error);

            aggregateResult.AddValidationResult(result, 0);

            aggregateResult.Success.Should().BeFalse();
        }

        [Test]
        [OrderingAutoData]
        public static void ToModelErrors_ReturnsExpectedResult(
            string field,
            string id,
            AggregateValidationResult aggregateResult)
        {
            var expectedKey = $"[0].{field}";
            var expectedErrors = new (string, string)[]
            {
                (expectedKey, id),
                (expectedKey, id),
            };

            var details = new ErrorDetails(id, field);
            var result = new ValidationResult(new[] { details, details });
            aggregateResult.AddValidationResult(result, 0);

            var actualErrors = aggregateResult.ToModelErrors();

            actualErrors.Should().BeEquivalentTo(expectedErrors);
        }
    }
}
