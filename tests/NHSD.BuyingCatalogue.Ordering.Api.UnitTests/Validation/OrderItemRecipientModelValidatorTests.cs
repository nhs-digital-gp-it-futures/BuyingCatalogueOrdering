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
            var model = new OrderItemRecipientModel { Name = new string('A', 257) };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.Name)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_NameIsValid_DoesNotHaveError(
            OrderItemRecipientModelValidator validator)
        {
            var model = new OrderItemRecipientModel { Name = new string('A', 256) };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Name);
        }

        [Test]
        [InlineAutoData(null)]
        [InlineAutoData("")]
        [InlineAutoData("\t")]
        public static void Validate_OdsCodeIsEmpty_HasError(
            string odsCode,
            OrderItemRecipientModelValidator validator)
        {
            var model = new OrderItemRecipientModel { OdsCode = odsCode };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.OdsCode)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.OdsCode)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_OdsCodeIsTooLong_HasError(
            OrderItemRecipientModelValidator validator)
        {
            var model = new OrderItemRecipientModel { OdsCode = new string('A', 9) };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.OdsCode)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.OdsCode)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_OdsCodeIsValid_DoesNotHaveError(
            OrderItemRecipientModelValidator validator)
        {
            var model = new OrderItemRecipientModel { OdsCode = new string('A', 8) };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.OdsCode);
        }

        [Test]
        [AutoData]
        public static void Validate_QuantityIsNull_HasError(
            OrderItemRecipientModelValidator validator)
        {
            var model = new OrderItemRecipientModel();

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.Quantity)}Required");
        }

        [Test]
        [InlineAutoData(0)]
        [InlineAutoData(-1)]
        public static void Validate_QuantityIsZeroOrLess_HasError(
            int quantity,
            OrderItemRecipientModelValidator validator)
        {
            var model = new OrderItemRecipientModel { Quantity = quantity };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.Quantity)}GreaterThanZero");
        }

        [Test]
        [AutoData]
        public static void Validate_QuantityIsMaxInt_HasError(
            OrderItemRecipientModelValidator validator)
        {
            var model = new OrderItemRecipientModel { Quantity = int.MaxValue };

            var result = validator.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage($"{nameof(OrderItemRecipientModel.Quantity)}LessThanMax");
        }

        [Test]
        [InlineAutoData(1)]
        [InlineAutoData(int.MaxValue - 1)]
        public static void Validate_QuantityIsValid_DoesNotHaveError(
            int quantity,
            OrderItemRecipientModelValidator validator)
        {
            var model = new OrderItemRecipientModel { Quantity = quantity };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Quantity);
        }
    }
}
