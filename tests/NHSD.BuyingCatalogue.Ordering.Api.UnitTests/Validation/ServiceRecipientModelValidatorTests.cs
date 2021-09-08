using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentValidation;
using FluentValidation.TestHelper;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Validation
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class ServiceRecipientModelValidatorTests
    {
        static ServiceRecipientModelValidatorTests()
        {
            ValidatorOptions.Global.DisplayNameResolver = FluentValidationOptions.DisplayNameResolver;
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_OdsCodeIsEmpty_HasError(
            string odsCode,
            ServiceRecipientModelValidator validator)
        {
            var model = new ServiceRecipientModel { OdsCode = odsCode };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.OdsCode)
                .WithErrorMessage($"{nameof(ServiceRecipientModel.OdsCode)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_OdsCodeIsTooLong_HasError(
            ServiceRecipientModelValidator validator)
        {
            var model = new ServiceRecipientModel { OdsCode = new string('1', 9) };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.OdsCode)
                .WithErrorMessage($"{nameof(ServiceRecipientModel.OdsCode)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_OdsCodeIsValid_DoesNotHaveError(
            ServiceRecipientModelValidator validator)
        {
            var model = new ServiceRecipientModel { OdsCode = new string('1', 8) };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.OdsCode);
        }
    }
}
