using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentValidation;
using FluentValidation.TestHelper;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Validation
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class UpdateOrderItemValidatorTests
    {
        static UpdateOrderItemValidatorTests()
        {
            ValidatorOptions.Global.DisplayNameResolver = FluentValidationOptions.DisplayNameResolver;
        }

        [Test]
        [AutoData]
        public static void Validate_QuantityIsNull_HasError(
            UpdateOrderItemModelValidator<UpdateOrderItemModel> validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Quantity, null as int?)
                .WithErrorMessage($"{nameof(UpdateOrderItemModel.Quantity)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_QuantityIsZero_HasError(
            UpdateOrderItemModelValidator<UpdateOrderItemModel> validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Quantity, 0)
                .WithErrorMessage($"{nameof(UpdateOrderItemModel.Quantity)}GreaterThanZero");
        }

        [Test]
        [AutoData]
        public static void Validate_QuantityIsMaxInt_HasError(
            UpdateOrderItemModelValidator<UpdateOrderItemModel> validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Quantity, int.MaxValue)
                .WithErrorMessage($"{nameof(UpdateOrderItemModel.Quantity)}LessThanMax");
        }

        [Test]
        [InlineAutoData(1)]
        [InlineAutoData(int.MaxValue - 1)]
        public static void Validate_QuantityIsValid_DoesNotHaveError(
            int quantity,
            UpdateOrderItemModelValidator<UpdateOrderItemModel> validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.Quantity, quantity);
        }

        [Test]
        [AutoData]
        public static void Validate_PriceIsNull_HasError(
            UpdateOrderItemModelValidator<UpdateOrderItemModel> validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Price, null as decimal?)
                .WithErrorMessage($"{nameof(UpdateOrderItemModel.Price)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_PriceIsLessThanZero_HasError(
            UpdateOrderItemModelValidator<UpdateOrderItemModel> validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Price, -0.01m)
                .WithErrorMessage($"{nameof(UpdateOrderItemModel.Price)}GreaterThanOrEqualToZero");
        }

        [Test]
        [AutoData]
        public static void Validate_PriceIsMaxPrice_HasError(
            UpdateOrderItemModelValidator<UpdateOrderItemModel> validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Price, UpdateOrderItemModelValidator<UpdateOrderItemModel>.MaxPrice + 0.001m)
                .WithErrorMessage($"{nameof(UpdateOrderItemModel.Price)}LessThanMax");
        }

        [TestCaseSource(nameof(PriceIsValidTestCases))]
        public static void Validate_PriceIsValid_DoesNotHaveError(decimal price)
        {
            var validator = new UpdateOrderItemModelValidator<UpdateOrderItemModel>();

            validator.ShouldNotHaveValidationErrorFor(m => m.Price, price);
        }

        private static IEnumerable<ITestCaseData> PriceIsValidTestCases()
        {
            yield return new TestCaseData(0.00m);
            yield return new TestCaseData(UpdateOrderItemModelValidator<UpdateOrderItemModel>.MaxPrice - 0.001m);
        }
    }
}
