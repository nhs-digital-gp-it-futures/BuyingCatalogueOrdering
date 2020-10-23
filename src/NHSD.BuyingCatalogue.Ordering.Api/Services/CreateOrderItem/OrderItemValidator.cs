using System;
using System.Collections.Generic;
using EnumsNET;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public sealed class OrderItemValidator : ICreateOrderItemValidator, IUpdateOrderItemValidator
    {
        private readonly ValidationSettings validationSettings;

        public OrderItemValidator(ValidationSettings validationSettings)
        {
            this.validationSettings = validationSettings ?? throw new ArgumentNullException(nameof(validationSettings));
        }

        public IEnumerable<ErrorDetails> Validate(CreateOrderItemRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Order is null)
                throw new ArgumentException("Request Order should not be null", nameof(request));

            if (request.Order.CommencementDate is null)
                throw new ArgumentException("Request Order Commencement Date should not be null");

            if (request.CatalogueItemType is null)
            {
                yield return new ErrorDetails("CatalogueItemTypeValidValue", "CatalogueItemType");
            }
            else if (request.CatalogueItemType.Equals(CatalogueItemType.Solution))
            {
                var errors = ValidateDeliveryDate(request.DeliveryDate, request.Order.CommencementDate.Value, request.CatalogueItemType);
                foreach (var error in errors)
                {
                    yield return error;
                }
            }

            var provisioningType = request.ProvisioningType;
            if (provisioningType is null)
            {
                yield return new ErrorDetails("ProvisioningTypeValidValue", nameof(CreateOrderItemSolutionModel.ProvisioningType));
            }
            else
            {
                var errors = ValidateEstimationPeriod(request.EstimationPeriod, provisioningType);
                foreach (var error in errors)
                {
                    yield return error;
                }
            }

            var cataloguePriceType = request.CataloguePriceType;

            if (cataloguePriceType is null)
            {
                yield return new ErrorDetails("TypeValidValue", nameof(CreateOrderItemSolutionModel.Type));
            }
            else if (cataloguePriceType.Equals(CataloguePriceType.Flat))
            {
                if (request.Price is null)
                {
                    yield return new ErrorDetails("PriceRequired", nameof(CreateOrderItemSolutionModel.Price));
                }
            }

            if (request.CurrencyCode != "GBP")
            {
                yield return new ErrorDetails("CurrencyCodeValidValue", nameof(CreateOrderItemSolutionModel.CurrencyCode));
            }
        }

        public IEnumerable<ErrorDetails> Validate(UpdateOrderItemRequest request, CatalogueItemType itemType, ProvisioningType? provisioningType)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (itemType is null)
                throw new ArgumentNullException(nameof(itemType));

            if (provisioningType is null)
                throw new ArgumentNullException(nameof(provisioningType));

            if (request.Order is null)
                throw new ArgumentException("Request Order should not be null", nameof(request));

            if (request.Order.CommencementDate is null)
                throw new ArgumentException("Request Order Commencement Date should not be null");

            var errors = new List<ErrorDetails>();

            errors.AddRange(ValidateDeliveryDate(request.DeliveryDate, request.Order.CommencementDate.Value, itemType));
            errors.AddRange(ValidateEstimationPeriod(request.EstimationPeriod, provisioningType));

            return errors;
        }

        private static IEnumerable<ErrorDetails> ValidateEstimationPeriod(TimeUnit? estimationPeriod, ProvisioningType? provisioningType)
        {
            if (!provisioningType.Equals(ProvisioningType.OnDemand))
            {
                yield break;
            }

            if (estimationPeriod is null)
            {
                yield return new ErrorDetails("EstimationPeriodRequiredIfVariableOnDemand", nameof(CreateOrderItemSolutionModel.EstimationPeriod));
            }
            else
            {
                if (!estimationPeriod.Value.IsDefined())
                {
                    yield return new ErrorDetails("EstimationPeriodValidValue", nameof(CreateOrderItemSolutionModel.EstimationPeriod));
                }
            }
        }

        private IEnumerable<ErrorDetails> ValidateDeliveryDate(DateTime? deliveryDate, DateTime commencementDate, CatalogueItemType itemType)
        {
            if (!itemType.Equals(CatalogueItemType.Solution))
            {
                yield break;
            }

            if (deliveryDate is null)
            {
                yield return new ErrorDetails("DeliveryDateRequired", nameof(CreateOrderItemSolutionModel.DeliveryDate));
            }
            else if (!IsDeliveryDateWithinWindow(deliveryDate.Value, commencementDate))
            {
                yield return new ErrorDetails("DeliveryDateOutsideDeliveryWindow", nameof(CreateOrderItemSolutionModel.DeliveryDate));
            }
        }

        private bool IsDeliveryDateWithinWindow(DateTime deliveryDate, DateTime commencementDate)
        {
            return deliveryDate >= commencementDate
                   && deliveryDate <= commencementDate.AddDays(validationSettings.MaxDeliveryDateWeekOffset * 7);
        }
    }
}
