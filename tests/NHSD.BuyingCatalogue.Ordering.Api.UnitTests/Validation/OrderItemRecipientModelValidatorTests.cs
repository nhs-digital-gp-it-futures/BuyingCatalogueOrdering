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
    internal static class OrderItemRecipientModelValidatorTests
    {
        static OrderItemRecipientModelValidatorTests()
        {
            ValidatorOptions.Global.DisplayNameResolver = FluentValidationOptions.DisplayNameResolver;
        }

        [Test]
        [AutoData]
        public static void Validate_NameIsTooLong_HasError(
            OrderItemRecipientModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Name, new string('A', 257))
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.Name)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_NameIsValid_DoesNotHaveError(
            OrderItemRecipientModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.Name, new string('A', 256));
        }

        [Test]
        [InlineAutoData(null)]
        [InlineAutoData("")]
        [InlineAutoData("\t")]
        public static void Validate_OdsCodeIsEmpty_HasError(
            string odsCode,
            OrderItemRecipientModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.OdsCode, odsCode)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.OdsCode)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_OdsCodeIsTooLong_HasError(
            OrderItemRecipientModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.OdsCode, new string('A', 9))
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.OdsCode)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_OdsCodeIsValid_DoesNotHaveError(
            OrderItemRecipientModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.OdsCode, new string('A', 8));
        }

        [Test]
        [AutoData]
        public static void Validate_QuantityIsNull_HasError(
            OrderItemRecipientModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Quantity, (int?)null)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.Quantity)}Required");
        }

        [Test]
        [InlineAutoData(0)]
        [InlineAutoData(-1)]
        public static void Validate_QuantityIsZeroOrLess_HasError(
            int quantity,
            OrderItemRecipientModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Quantity, quantity)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.Quantity)}GreaterThanZero");
        }

        [Test]
        [AutoData]
        public static void Validate_QuantityIsMaxInt_HasError(
            OrderItemRecipientModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Quantity, int.MaxValue)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.Quantity)}LessThanMax");
        }

        [Test]
        [InlineAutoData(1)]
        [InlineAutoData(int.MaxValue - 1)]
        public static void Validate_QuantityIsValid_DoesNotHaveError(
            int quantity,
            OrderItemRecipientModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.Quantity, quantity);
        }
    }
}
