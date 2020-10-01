using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Validation
{
    [TestFixture]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class DefaultDeliveryDateValidatorTests
    {
        [Test]
        [AutoData]
        public static void Validate_NullCommencementDate_ReturnsExpectedResult(
            DefaultDeliveryDateModel model,
            DefaultDeliveryDateValidator validator)
        {
            (bool isValid, ErrorsModel errors) = validator.Validate(model, null);

            isValid.Should().BeFalse();
            errors.Should().NotBeNull();
            errors.Errors.Should().NotBeNull();
            errors.Errors.Should().HaveCount(1);
            errors.Errors.First().Should().BeEquivalentTo(DefaultDeliveryDateValidator.CommencementDateRequired);
        }

        [Test]
        [AutoData]
        public static void Validate_DateBeforeCommencementDate_ReturnsExpectedResult(
            DateTime commencementDate,
            DefaultDeliveryDateModel model,
            DefaultDeliveryDateValidator validator)
        {
            model.DeliveryDate = commencementDate.AddDays(-1);
            (bool isValid, ErrorsModel errors) = validator.Validate(model, commencementDate);

            isValid.Should().BeFalse();
            errors.Should().NotBeNull();
            errors.Errors.Should().NotBeNull();
            errors.Errors.Should().HaveCount(1);
            errors.Errors.First().Should().BeEquivalentTo(DefaultDeliveryDateValidator.OutsideWindow);
        }

        [Test]
        [AutoData]
        public static void Validate_DateOutsideWindow_ReturnsExpectedResult(
            DateTime commencementDate,
            [Frozen] ValidationSettings validationSettings,
            DefaultDeliveryDateModel model,
            DefaultDeliveryDateValidator validator)
        {
            model.DeliveryDate = commencementDate.AddDays(validationSettings.MaxDeliveryDateOffsetInDays + 1);
            (bool isValid, ErrorsModel errors) = validator.Validate(model, commencementDate);

            isValid.Should().BeFalse();
            errors.Should().NotBeNull();
            errors.Errors.Should().NotBeNull();
            errors.Errors.Should().HaveCount(1);
            errors.Errors.First().Should().BeEquivalentTo(DefaultDeliveryDateValidator.OutsideWindow);
        }

        [Test]
        [AutoData]
        public static void Validate_WithinLimit_ReturnsExpectedResult(
            DateTime commencementDate,
            [Frozen] ValidationSettings validationSettings,
            DefaultDeliveryDateModel model,
            DefaultDeliveryDateValidator validator)
        {
            model.DeliveryDate = commencementDate.AddDays(validationSettings.MaxDeliveryDateOffsetInDays - 1);
            (bool isValid, ErrorsModel errors) = validator.Validate(model, commencementDate);

            isValid.Should().BeTrue();
            errors.Should().NotBeNull();
            errors.Errors.Should().NotBeNull();
            errors.Errors.Should().HaveCount(0);
        }

        [Test]
        [AutoData]
        public static void Validate_DateAtLimit_ReturnsExpectedResult(
            DateTime commencementDate,
            [Frozen] ValidationSettings validationSettings,
            DefaultDeliveryDateModel model,
            DefaultDeliveryDateValidator validator)
        {
            model.DeliveryDate = commencementDate.AddDays(validationSettings.MaxDeliveryDateOffsetInDays);
            (bool isValid, ErrorsModel errors) = validator.Validate(model, commencementDate);

            isValid.Should().BeTrue();
            errors.Should().NotBeNull();
            errors.Errors.Should().NotBeNull();
            errors.Errors.Should().HaveCount(0);
        }
    }
}
