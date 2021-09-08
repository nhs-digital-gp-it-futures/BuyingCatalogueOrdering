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
            var model = new OrderDescriptionModel { Description = description };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage($"{nameof(OrderDescriptionModel.Description)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_DescriptionIsTooLong_HasError(
            OrderDescriptionModelValidator validator)
        {
            var model = new OrderDescriptionModel { Description = new string('A', 101) };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage($"{nameof(OrderDescriptionModel.Description)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_DescriptionIsValid_DoesNotHaveError(
            OrderDescriptionModelValidator validator)
        {
            var model = new OrderDescriptionModel { Description = new string('A', 100) };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Description);
        }
    }
}
