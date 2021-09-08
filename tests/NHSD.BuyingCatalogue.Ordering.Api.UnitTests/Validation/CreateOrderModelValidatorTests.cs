using System;
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
    internal static class CreateOrderModelValidatorTests
    {
        static CreateOrderModelValidatorTests()
        {
            ValidatorOptions.Global.DisplayNameResolver = FluentValidationOptions.DisplayNameResolver;
        }

        [Test]
        [InlineAutoData(null)]
        [InlineAutoData("")]
        [InlineAutoData("\t")]
        public static void Validate_DescriptionIsEmpty_HasError(
            string description,
            CreateOrderModelValidator validator)
        {
            var model = new CreateOrderModel { Description = description };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage($"{nameof(CreateOrderModel.Description)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_DescriptionIsTooLong_HasError(
            CreateOrderModelValidator validator)
        {
            var model = new CreateOrderModel { Description = new string('A', 101) };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage($"{nameof(CreateOrderModel.Description)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_DescriptionIsValid_DoesNotHaveError(
            CreateOrderModelValidator validator)
        {
            var model = new CreateOrderModel { Description = new string('A', 10) };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Description);
        }

        [Test]
        [AutoData]
        public static void Validate_OrganisationIdIsEmptyGuid_HasError(
            CreateOrderModelValidator validator)
        {
            var model = new CreateOrderModel { OrganisationId = Guid.Empty };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.OrganisationId)
                .WithErrorMessage($"{nameof(CreateOrderModel.OrganisationId)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_OrganisationIdIsValid_DoesNotHaveError(
            CreateOrderModel model,
            CreateOrderModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.OrganisationId);
        }
    }
}
