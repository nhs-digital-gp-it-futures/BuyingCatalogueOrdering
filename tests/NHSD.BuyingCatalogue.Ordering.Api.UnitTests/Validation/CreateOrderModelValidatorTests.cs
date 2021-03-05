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
            validator
                .ShouldHaveValidationErrorFor(m => m.Description, description)
                .WithErrorMessage($"{nameof(CreateOrderModel.Description)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_DescriptionIsTooLong_HasError(
            CreateOrderModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Description, new string('A', 101))
                .WithErrorMessage($"{nameof(CreateOrderModel.Description)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_DescriptionIsValid_DoesNotHaveError(
            CreateOrderModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.Description, new string('A', 100));
        }

        [Test]
        [AutoData]
        public static void Validate_OrganisationIdIsEmptyGuid_HasError(
            CreateOrderModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.OrganisationId, Guid.Empty)
                .WithErrorMessage($"{nameof(CreateOrderModel.OrganisationId)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_OrganisationIdIsValid_DoesNotHaveError(
            Guid id,
            CreateOrderModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.OrganisationId, id);
        }
    }
}
