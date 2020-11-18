using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Validation
{
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class ServiceRecipientModelValidatorTests
    {
        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_OdsCodeIsEmpty_HasError(
            string odsCode,
            ServiceRecipientModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.OdsCode, odsCode)
                .WithErrorMessage($"{nameof(ServiceRecipientModel.OdsCode)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_OdsCodeIsTooLong_HasError(
            ServiceRecipientModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.OdsCode, new string('1', 9))
                .WithErrorMessage($"{nameof(ServiceRecipientModel.OdsCode)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_OdsCodeIsValid_DoesNotHaveError(
            ServiceRecipientModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.OdsCode, new string('1', 8));
        }
    }
}
