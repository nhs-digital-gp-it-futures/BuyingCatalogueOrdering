using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
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
        [CommonInlineAutoData(nameof(CatalogueItemType.AdditionalService))]
        [CommonInlineAutoData(nameof(CatalogueItemType.AssociatedService))]
        [CommonInlineAutoData(nameof(CatalogueItemType.Solution))]
        public static void Validate_CatalogueItemTypeIsValid_DoesNotHaveError(
            string catalogueItemType,
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.CatalogueItemType, catalogueItemType);
        }

        [Test]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
        [CommonInlineAutoData("InvalidCatalogueItemType")]
        public static void Validate_CatalogueItemTypeIsNotValid_DoesNotHaveErrorsForDependentRules(
            string catalogueItemType,
            CreateOrderItemModelValidator validator)
        {
            var model = new CreateOrderItemModel { CatalogueItemType = catalogueItemType };

            validator.ShouldNotHaveValidationErrorFor(m => m.CatalogueSolutionId, model);
            validator.ShouldNotHaveValidationErrorFor(m => m.ServiceRecipients, model);
            validator.ShouldNotHaveValidationErrorFor(m => m.TimeUnit, model);
        }

        [Test]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
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
        [CommonInlineAutoData(nameof(ProvisioningType.Declarative))]
        [CommonInlineAutoData(nameof(ProvisioningType.OnDemand))]
        [CommonInlineAutoData(nameof(ProvisioningType.Patient))]
        public static void Validate_ProvisioningTypeIsValid_DoesNotHaveError(
            string provisioningType,
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.ProvisioningType, provisioningType);
        }

        [Test]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
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
        [CommonInlineAutoData(nameof(CataloguePriceType.Flat))]
        [CommonInlineAutoData(nameof(CataloguePriceType.Tiered))]
        public static void Validate_TypeIsValid_DoesNotHaveError(
            string type,
            CreateOrderItemModelValidator validator)
        {
            validator.ShouldNotHaveValidationErrorFor(m => m.Type, type);
        }

        [Test]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
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
        public static void Validate_CatalogueSolutionIdIsNotCatalogueItemId_IsAdditionalService_HasError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = nameof(CatalogueItemType.AdditionalService);
            model.CatalogueSolutionId = new string('1', 14);

            validator
                .ShouldHaveValidationErrorFor(m => m.CatalogueSolutionId, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.CatalogueSolutionId)}ValidValue");
        }

        [Test]
        [AutoData]
        public static void Validate_CatalogueSolutionIdIsValid_IsAdditionalService_DoesNotHaveError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = nameof(CatalogueItemType.AdditionalService);
            model.CatalogueSolutionId = new CatalogueItemId(1, "001").ToString();

            validator.ShouldNotHaveValidationErrorFor(m => m.CatalogueSolutionId, model);
        }

        [Test]
        [CommonInlineAutoData(nameof(CatalogueItemType.AssociatedService))]
        [CommonInlineAutoData(nameof(CatalogueItemType.Solution))]
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
        [CommonInlineAutoData(nameof(CatalogueItemType.AdditionalService))]
        [CommonInlineAutoData(nameof(CatalogueItemType.Solution))]
        public static void Validate_ServiceRecipientsIsNull_IsNotAssociatedService_HasError(
            string catalogueItemType,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = catalogueItemType;
            model.ServiceRecipients = null;

            validator
                .ShouldHaveValidationErrorFor(m => m.ServiceRecipients, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.ServiceRecipients)}Required");
        }

        [Test]
        [CommonInlineAutoData(nameof(CatalogueItemType.AdditionalService))]
        [CommonInlineAutoData(nameof(CatalogueItemType.Solution))]
        public static void Validate_ServiceRecipientsIsNotNull_IsNotAssociatedService_DoesNotHaveError(
            string catalogueItemType,
            List<OrderItemRecipientModel> recipients,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = catalogueItemType;
            model.ServiceRecipients = recipients;

            validator.ShouldNotHaveValidationErrorFor(m => m.ServiceRecipients, model);
        }

        [Test]
        [CommonAutoData]
        public static void Validate_ServiceRecipientsContainsDuplicates_HasError(
            OrderItemRecipientModel recipientModel,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.CatalogueItemType = nameof(CatalogueItemType.Solution);
            model.ServiceRecipients = new List<OrderItemRecipientModel> { recipientModel, recipientModel };

            validator
                .ShouldHaveValidationErrorFor(m => m.ServiceRecipients, model)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.ServiceRecipients)}ContainsDuplicates");
        }

        [Test]
        [CommonInlineAutoData(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Declarative))]
        [CommonInlineAutoData(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Patient))]
        [CommonInlineAutoData(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Declarative))]
        [CommonInlineAutoData(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Patient))]
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
        [CommonInlineAutoData(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Declarative))]
        [CommonInlineAutoData(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Patient))]
        [CommonInlineAutoData(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Declarative))]
        [CommonInlineAutoData(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Patient))]
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
            var recipient = new OrderItemRecipientModel();
            model.CatalogueItemType = nameof(CatalogueItemType.Solution);
            model.ServiceRecipients = new List<OrderItemRecipientModel> { recipient };

            var expectedPropertyName =
                $"{nameof(CreateOrderItemModel.ServiceRecipients)}[0].{nameof(OrderItemRecipientModel.DeliveryDate)}";

            var result = validator.Validate(model);
            var failure = result.Errors.FirstOrDefault(e => e.PropertyName == expectedPropertyName);

            Assert.NotNull(failure);
            failure.ErrorMessage.Should().Be($"{nameof(OrderItemRecipientModel.DeliveryDate)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_DeliveryDateIsValid_IsSolution_DoesNotHaveError(
            DateTime deliveryDate,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            var recipient = new OrderItemRecipientModel { DeliveryDate = deliveryDate };
            model.CatalogueItemType = nameof(CatalogueItemType.Solution);
            model.ServiceRecipients = new List<OrderItemRecipientModel> { recipient };

            var expectedPropertyName =
                $"{nameof(CreateOrderItemModel.ServiceRecipients)}[0].{nameof(OrderItemRecipientModel.DeliveryDate)}";

            var result = validator.Validate(model);
            var failure = result.Errors.FirstOrDefault(e => e.PropertyName == expectedPropertyName);

            failure.Should().BeNull();
        }

        [Test]
        [AutoData]
        public static void Validate_DeliveryDateIsValid_IsAdditionalService_DoesNotHaveError(
            DateTime deliveryDate,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            var recipient = new OrderItemRecipientModel { DeliveryDate = deliveryDate };
            model.CatalogueItemType = nameof(CatalogueItemType.AdditionalService);
            model.ServiceRecipients = new List<OrderItemRecipientModel> { recipient };

            var expectedPropertyName =
                $"{nameof(CreateOrderItemModel.ServiceRecipients)}[0].{nameof(OrderItemRecipientModel.DeliveryDate)}";

            var result = validator.Validate(model);
            var failure = result.Errors.FirstOrDefault(e => e.PropertyName == expectedPropertyName);

            failure.Should().BeNull();
        }

        [Test]
        [CommonInlineAutoData(nameof(CatalogueItemType.AssociatedService))]
        public static void Validate_DeliveryDateIsNull_IsAssociatedService_DoesNotHaveError(
            string catalogueItemType,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            var recipient = new OrderItemRecipientModel();
            model.CatalogueItemType = catalogueItemType;
            model.ServiceRecipients = new List<OrderItemRecipientModel> { recipient };

            var expectedPropertyName =
                $"{nameof(CreateOrderItemModel.ServiceRecipients)}[0].{nameof(OrderItemRecipientModel.DeliveryDate)}";

            var result = validator.Validate(model);
            var failure = result.Errors.FirstOrDefault(e => e.PropertyName == expectedPropertyName);

            failure.Should().BeNull();
        }

        [Test]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
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
        [CommonInlineAutoData(nameof(TimeUnit.PerMonth))]
        [CommonInlineAutoData(nameof(TimeUnit.PerYear))]
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
        [CommonInlineAutoData(nameof(ProvisioningType.Declarative), null)]
        [CommonInlineAutoData(nameof(ProvisioningType.Declarative), "")]
        [CommonInlineAutoData(nameof(ProvisioningType.Declarative), "\t")]
        [CommonInlineAutoData(nameof(ProvisioningType.Patient), null)]
        [CommonInlineAutoData(nameof(ProvisioningType.Patient), "")]
        [CommonInlineAutoData(nameof(ProvisioningType.Patient), "\t")]
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
        [CommonInlineAutoData(nameof(ProvisioningType.Declarative))]
        [CommonInlineAutoData(nameof(ProvisioningType.Patient))]
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
        [CommonInlineAutoData(nameof(ProvisioningType.Declarative))]
        [CommonInlineAutoData(nameof(ProvisioningType.Patient))]
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

        [Test]
        [AutoData]
        public static void Validate_PriceIsNull_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Price, null as decimal?)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.Price)}Required");
        }

        [Test]
        [AutoData]
        public static void Validate_PriceIsLessThanZero_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Price, -0.01m)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.Price)}GreaterThanOrEqualToZero");
        }

        [Test]
        [AutoData]
        public static void Validate_PriceIsMaxPrice_HasError(
            CreateOrderItemModelValidator validator)
        {
            validator
                .ShouldHaveValidationErrorFor(m => m.Price, CreateOrderItemModelValidator.MaxPrice + 0.001m)
                .WithErrorMessage($"{nameof(CreateOrderItemModel.Price)}LessThanMax");
        }

        [TestCaseSource(nameof(PriceIsValidTestCases))]
        public static void Validate_PriceIsValid_DoesNotHaveError(decimal price)
        {
            var validator = new CreateOrderItemModelValidator();

            validator.ShouldNotHaveValidationErrorFor(m => m.Price, price);
        }

        private static IEnumerable<ITestCaseData> PriceIsValidTestCases()
        {
            yield return new TestCaseData(0.00m);
            yield return new TestCaseData(CreateOrderItemModelValidator.MaxPrice - 0.001m);
        }
    }
}
