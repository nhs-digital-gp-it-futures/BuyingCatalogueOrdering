using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal sealed class CreateOrderItemModelValidator : AbstractValidator<CreateOrderItemModel>
    {
        internal const decimal MaxPrice = 999999999999999.999m;

        public CreateOrderItemModelValidator()
        {
            RuleFor(m => m.CatalogueItemType)
                .Required()
                .IsEnumName<CreateOrderItemModel, CatalogueItemType>()
                .DependentRules(
                    () =>
                    {
                        RuleFor(m => m.CatalogueSolutionId)
                            .Required()
                            .MaxLength(14)
                            .When(IsAdditionalService);

                        Unless(IsAssociatedService, () =>
                        {
                            RuleFor(m => m.TimeUnit)
                                .Required()
                                .When(HasProvisioningType)
                                .Unless(IsOnDemand);
                        });

                        RuleFor(m => m.ServiceRecipients).Required();
                        RuleForEach(m => m.ServiceRecipients).SetValidator(new OrderItemRecipientModelValidator());
                        RuleForEach(m => m.ServiceRecipients)
                            .ChildRules(v => v.RuleFor(r => r.DeliveryDate).Required())
                            .When(IsSolution);

                        RuleFor(m => m.ServiceRecipients)
                            .Must(NotContainDuplicates)
                            .WithMessage("{PropertyName}ContainsDuplicates");
                    });

            RuleFor(m => m.ProvisioningType)
                .Required()
                .IsEnumName<CreateOrderItemModel, ProvisioningType>();

            RuleFor(m => m.CatalogueItemName)
                .Required()
                .MaxLength(255);

            RuleFor(m => m.Type)
                .Required()
                .IsEnumName<CreateOrderItemModel, CataloguePriceType>();

            RuleFor(m => m.CurrencyCode)
                .Required()
                .Matches("GBP").WithMessage("{PropertyName}ValidValue");

            RuleFor(m => m.ItemUnit).Required();

            RuleFor(m => m.EstimationPeriod)
                .Required("{PropertyName}RequiredIfVariableOnDemand")
                .Must(BeValidEstimationPeriod)
                .WithValidValueMessage()
                .When(HasProvisioningType)
                .When(IsOnDemand);

            RuleFor(m => m.Price)
                .Required()
                .GreaterThanOrEqualTo(0.00m).WithMessage("{PropertyName}GreaterThanOrEqualToZero")
                .LessThanOrEqualToMax(MaxPrice);
        }

        private static bool NotContainDuplicates(IReadOnlyList<OrderItemRecipientModel> recipients)
        {
            var recipientCodes = new HashSet<string>();
            return recipients?.All(recipient => recipientCodes.Add(recipient.OdsCode)) ?? true;
        }

        private static bool BeValidEstimationPeriod(string estimationPeriod)
        {
            return OrderingEnums.ParseTimeUnit(estimationPeriod) != 0;
        }

        private static bool HasProvisioningType(CreateOrderItemModel model)
        {
            return Enum.TryParse<ProvisioningType>(model.ProvisioningType, true, out _);
        }

        private static bool IsAdditionalService(CreateOrderItemModel model)
        {
            return IsCatalogueItemType(model, nameof(CatalogueItemType.AdditionalService));
        }

        private static bool IsAssociatedService(CreateOrderItemModel model)
        {
            return IsCatalogueItemType(model, nameof(CatalogueItemType.AssociatedService));
        }

        private static bool IsSolution(CreateOrderItemModel model)
        {
            return IsCatalogueItemType(model, nameof(CatalogueItemType.Solution));
        }

        private static bool IsCatalogueItemType(CreateOrderItemModel model, string catalogueItemType)
        {
            return model.CatalogueItemType.EqualsOrdinalIgnoreCase(catalogueItemType);
        }

        private static bool IsOnDemand(CreateOrderItemModel model)
        {
            return model.ProvisioningType.EqualsOrdinalIgnoreCase(nameof(ProvisioningType.OnDemand));
        }
    }
}
