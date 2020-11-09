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
        public static void Success_SuccessfulResultsOnly_IsTrue(AggregateValidationResult aggregateResult)
        {
            aggregateResult.AddValidationResult(new ValidationResult(Array.Empty<ErrorDetails>()), 0);

            aggregateResult.Success.Should().BeTrue();
        }

        [Test]
        [OrderingAutoData]
        public static void Success_FailedResults_IsFalse(
            ValidationResult result,
            AggregateValidationResult aggregateResult)
        {
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
                ( expectedKey, id ),
                ( expectedKey, id ),
            };

            var details = new ErrorDetails(id, field);
            var result = new ValidationResult(new[] { details, details });
            aggregateResult.AddValidationResult(result, 0);

            var actualErrors = aggregateResult.ToModelErrors();

            actualErrors.Should().BeEquivalentTo(expectedErrors);
        }
    }
}
