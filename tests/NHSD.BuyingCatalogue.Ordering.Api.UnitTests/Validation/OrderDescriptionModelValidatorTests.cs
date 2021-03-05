using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentValidation;
using FluentValidation.TestHelper;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Validation
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderDescriptionModelValidatorTests
    {
        static OrderDescriptionModelValidatorTests()
        {
            ValidatorOptions.Global.DisplayNameResolver = FluentValidationOptions.DisplayNameResolver;
        }

        [Test]
        [InlineAutoData(null)]
        [InlineAutoData("")]
        [InlineAutoData("\t")]
        public static void Validate_DescriptionIsEmpty_HasError(
            string description,
            OrderDescriptionModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Description, description)
                .WithErrorMessage($"{nameof(OrderDescriptionModel.Description)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_DescriptionIsTooLong_HasError(
            OrderDescriptionModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Description, new string('A', 101))
                .WithErrorMessage($"{nameof(OrderDescriptionModel.Description)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_DescriptionIsValid_DoesNotHaveError(
            OrderDescriptionModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.Description, new string('A', 100));
        }
    }
}
