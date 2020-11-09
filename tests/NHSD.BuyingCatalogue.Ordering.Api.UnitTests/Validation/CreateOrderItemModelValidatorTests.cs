using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentValidation;
using FluentValidation.TestHelper;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Validation
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemModelValidatorTests
    {
        static CreateOrderItemModelValidatorTests()
        {
            ValidatorOptions.Global.DisplayNameResolver = FluentValidationOptions.DisplayNameResolver;
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_CatalogueItemTypeIsEmpty_HasError(
            string catalogueItemType,
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.CatalogueItemType, catalogueItemType)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CatalogueItemType)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_CatalogueItemTypeIsNotValid_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.CatalogueItemType, "InvalidCatalogueItemType")
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CatalogueItemType)}ValidValue");
        }

        [Test]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AdditionalService))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AssociatedService))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.Solution))]
        public static void Validate_CatalogueItemTypeIsValid_DoesNotHaveError(
            string catalogueItemType,
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.CatalogueItemType, catalogueItemType);
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        [OrderingInlineAutoData("InvalidCatalogueItemType")]
        public static void Validate_CatalogueItemTypeIsNotValid_DoesNotHaveErrorsForDependentRules(
            string catalogueItemType,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = catalogueItemType;

            validator.ShouldNotHaveValidationErrorFor(m => m.CatalogueSolutionId, model);
            validator.ShouldNotHaveValidationErrorFor(m => m.DeliveryDate, model);
            validator.ShouldNotHaveValidationErrorFor(m => m.ServiceRecipient, model);
            validator.ShouldNotHaveValidationErrorFor(m => m.TimeUnit, model);
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_CatalogueItemIdIsEmpty_HasError(
            string catalogueItemId,
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.CatalogueItemId, catalogueItemId)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CatalogueItemId)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_CatalogueItemIdIsTooLong_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.CatalogueItemId, new string('1', 15))
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CatalogueItemId)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_CatalogueItemIdIsValid_DoesNotHaveError(
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.CatalogueItemId, new string('1', 14));
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_CatalogueItemNameIsEmpty_HasError(
            string catalogueItemName,
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.CatalogueItemName, catalogueItemName)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CatalogueItemName)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_CatalogueItemNameIsTooLong_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.CatalogueItemName, new string('A', 256))
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CatalogueItemName)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_CatalogueItemNameIsValid_DoesNotHaveError(
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.CatalogueItemName, new string('A', 255));
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_ProvisioningTypeIsEmpty_HasError(
            string provisioningType,
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.ProvisioningType, provisioningType)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.ProvisioningType)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_ProvisioningTypeIsNotValid_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.ProvisioningType, "InvalidProvisioningType")
                .WithErrorMessage($"{nameof(CreateOrderItemModel.ProvisioningType)}ValidValue");
        }

        [Test]
        [OrderingInlineAutoData(nameof(ProvisioningType.Declarative))]
        [OrderingInlineAutoData(nameof(ProvisioningType.OnDemand))]
        [OrderingInlineAutoData(nameof(ProvisioningType.Patient))]
        public static void Validate_ProvisioningTypeIsValid_DoesNotHaveError(
            string provisioningType,
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.ProvisioningType, provisioningType);
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_TypeIsEmpty_HasError(
            string type,
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Type, type)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.Type)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_TypeIsNotValid_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Type, "InvalidType")
                .WithErrorMessage($"{nameof(CreateOrderItemModel.Type)}ValidValue");
        }

        [Test]
        [OrderingInlineAutoData(nameof(CataloguePriceType.Flat))]
        [OrderingInlineAutoData(nameof(CataloguePriceType.Tiered))]
        public static void Validate_TypeIsValid_DoesNotHaveError(
            string type,
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.Type, type);
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_CurrencyCodeIsEmpty_HasError(
            string currencyCode,
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.CurrencyCode, currencyCode)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CurrencyCode)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_CurrencyCodeIsNotValid_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.CurrencyCode, "InvalidCurrencyCode")
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CurrencyCode)}ValidValue");
        }

        [Test]
        [AutoData]
        public static void Validate_CurrencyCodeIsValid_DoesNotHaveError(
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.CurrencyCode, "GBP");
        }

        [Test]
        [AutoData]
        public static void Validate_ItemUnitIsNull_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.ItemUnit, null as ItemUnitModel)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.ItemUnit)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_ItemUnitIsNotNull_DoesNotHaveError(
            ItemUnitModel itemUnit,
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.ItemUnit, itemUnit);
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_CatalogueSolutionIdIsEmpty_IsAdditionalService_HasError(
            string catalogueSolutionId,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = nameof(CatalogueItemType.AdditionalService);
            model.CatalogueSolutionId = catalogueSolutionId;

            validator
                .ShouldHaveValidationErrorFor(m => m.CatalogueSolutionId, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CatalogueSolutionId)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_CatalogueSolutionIdIsTooLong_IsAdditionalService_HasError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = nameof(CatalogueItemType.AdditionalService);
            model.CatalogueSolutionId = new string('1', 15);

            validator
                .ShouldHaveValidationErrorFor(m => m.CatalogueSolutionId, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CatalogueSolutionId)}TooLong");
        }

        [Test]
        [AutoData]
        public static void Validate_CatalogueSolutionIdIsValid_IsAdditionalService_DoesNotHaveError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = nameof(CatalogueItemType.AdditionalService);
            model.CatalogueSolutionId = new string('1', 14);

            validator.ShouldNotHaveValidationErrorFor(m => m.CatalogueSolutionId, model);
        }

        [Test]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AssociatedService))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.Solution))]
        public static void Validate_CatalogueSolutionIdIsNull_IsNotAdditionalService_DoesNotHaveError(
            string catalogueItemType,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = catalogueItemType;
            model.CatalogueSolutionId = null;

            validator.ShouldNotHaveValidationErrorFor(m => m.CatalogueSolutionId, model);
        }

        [Test]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AdditionalService))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.Solution))]
        public static void Validate_ServiceRecipientIsNull_IsNotAssociatedService_HasError(
            string catalogueItemType,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = catalogueItemType;
            model.ServiceRecipient = null;

            validator
                .ShouldHaveValidationErrorFor(m => m.ServiceRecipient, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.ServiceRecipient)}Required");
        }

        [Test]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AdditionalService))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.Solution))]
        public static void Validate_ServiceRecipientIsNotNull_IsNotAssociatedService_DoesNotHaveError(
            string catalogueItemType,
            ServiceRecipientModel recipient,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = catalogueItemType;
            model.ServiceRecipient = recipient;

            validator.ShouldNotHaveValidationErrorFor(m => m.ServiceRecipient, model);
        }

        [Test]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Declarative))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Patient))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Declarative))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Patient))]
        public static void Validate_TimeUnitIsNull_IsNotAssociatedService_IsNotOnDemand_HasError(
            string catalogueItemType,
            string provisioningType,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = catalogueItemType;
            model.ProvisioningType = provisioningType;
            model.TimeUnit = null;

            validator
                .ShouldHaveValidationErrorFor(m => m.TimeUnit, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.TimeUnit)}Required");
        }

        [Test]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Declarative))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Patient))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Declarative))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Patient))]
        public static void Validate_TimeUnitIsNotNull_IsNotAssociatedService_IsNotOnDemand_DoesNotHaveError(
            string catalogueItemType,
            string provisioningType,
            TimeUnitModel timeUnit,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = catalogueItemType;
            model.ProvisioningType = provisioningType;
            model.TimeUnit = timeUnit;

            validator.ShouldNotHaveValidationErrorFor(m => m.TimeUnit, model);
        }

        [Test]
        [AutoData]
        public static void Validate_DeliveryDateIsNull_IsSolution_HasError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = nameof(CatalogueItemType.Solution);
            model.DeliveryDate = null;

            validator
                .ShouldHaveValidationErrorFor(m => m.DeliveryDate, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.DeliveryDate)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_DeliveryDateIsValid_IsSolution_DoesNotHaveError(
            DateTime deliveryDate,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = nameof(CatalogueItemType.Solution);
            model.DeliveryDate = deliveryDate;

            validator.ShouldNotHaveValidationErrorFor(m => m.DeliveryDate, model);
        }

        [Test]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AdditionalService))]
        [OrderingInlineAutoData(nameof(CatalogueItemType.AssociatedService))]
        public static void Validate_DeliveryDateIsNull_IsNotSolution_DoesNotHaveError(
            string catalogueItemType,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = catalogueItemType;
            model.DeliveryDate = null;

            validator.ShouldNotHaveValidationErrorFor(m => m.DeliveryDate, model);
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("")]
        [OrderingInlineAutoData("\t")]
        public static void Validate_EstimationPeriodIsEmpty_IsOnDemand_HasError(
            string estimationPeriod,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.ProvisioningType = nameof(ProvisioningType.OnDemand);
            model.EstimationPeriod = estimationPeriod;

            validator
                .ShouldHaveValidationErrorFor(m => m.EstimationPeriod, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.EstimationPeriod)}RequiredIfVariableOnDemand");
        }

        [Test]
        [AutoData]
        public static void Validate_EstimationPeriodIsNotValid_IsOnDemand_HasError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.ProvisioningType = nameof(ProvisioningType.OnDemand);
            model.EstimationPeriod = "InvalidTimeUnit";

            validator
                .ShouldHaveValidationErrorFor(m => m.EstimationPeriod, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.EstimationPeriod)}ValidValue");
        }

        [Test]
        [OrderingInlineAutoData(nameof(TimeUnit.PerMonth))]
        [OrderingInlineAutoData(nameof(TimeUnit.PerYear))]
        public static void Validate_EstimationPeriodIsValid_IsOnDemand_DoesNotHaveError(
            string estimationPeriod,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.ProvisioningType = nameof(ProvisioningType.OnDemand);
            model.EstimationPeriod = estimationPeriod;

            validator.ShouldNotHaveValidationErrorFor(m => m.EstimationPeriod, model);
        }

        [Test]
        [OrderingInlineAutoData(nameof(ProvisioningType.Declarative), null)]
        [OrderingInlineAutoData(nameof(ProvisioningType.Declarative), "")]
        [OrderingInlineAutoData(nameof(ProvisioningType.Declarative), "\t")]
        [OrderingInlineAutoData(nameof(ProvisioningType.Patient), null)]
        [OrderingInlineAutoData(nameof(ProvisioningType.Patient), "")]
        [OrderingInlineAutoData(nameof(ProvisioningType.Patient), "\t")]
        public static void Validate_EstimationPeriodIsEmpty_IsNotOnDemand_DoesNotHaveError(
            string provisioningType,
            string estimationPeriod,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.ProvisioningType = provisioningType;
            model.EstimationPeriod = estimationPeriod;

            validator.ShouldNotHaveValidationErrorFor(m => m.EstimationPeriod, model);
        }

        [Test]
        [OrderingInlineAutoData(nameof(ProvisioningType.Declarative))]
        [OrderingInlineAutoData(nameof(ProvisioningType.Patient))]
        public static void Validate_EstimationPeriodIsNotValid_IsNotOnDemand_DoesNotHaveError(
            string provisioningType,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.ProvisioningType = provisioningType;
            model.EstimationPeriod = "InvalidTimeUnit";

            validator.ShouldNotHaveValidationErrorFor(m => m.EstimationPeriod, model);
        }

        [Test]
        [OrderingInlineAutoData(nameof(ProvisioningType.Declarative))]
        [OrderingInlineAutoData(nameof(ProvisioningType.Patient))]
        public static void Validate_TimeUnitIsNull_IsNotOnDemand_HasError(
            string provisioningType,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.ProvisioningType = provisioningType;
            model.TimeUnit = null;

            validator
                .ShouldHaveValidationErrorFor(m => m.TimeUnit, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.TimeUnit)}Required");
        }

        [Test]
        [OrderingInlineAutoData(nameof(ProvisioningType.Declarative))]
        [OrderingInlineAutoData(nameof(ProvisioningType.Patient))]
        public static void Validate_TimeUnitIsNotNull_IsNotOnDemand_DoesNotHaveError(
            string provisioningType,
            TimeUnitModel timeUnit,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.ProvisioningType = provisioningType;
            model.TimeUnit = timeUnit;

            validator.ShouldNotHaveValidationErrorFor(m => m.TimeUnit, model);
        }

        [Test]
        [AutoData]
        public static void Validate_TimeUnitIsNull_IsOnDemand_DoesNotHaveError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.ProvisioningType = nameof(ProvisioningType.OnDemand);
            model.TimeUnit = null;

            validator.ShouldNotHaveValidationErrorFor(m => m.TimeUnit, model);
        }
    }
}
