using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public sealed class OrderItemValidator : ICreateOrderItemValidator, IUpdateOrderItemValidator
    {
        private readonly ValidationSettings _validationSettings;

        public OrderItemValidator(ValidationSettings validationSettings)
        {
            _validationSettings = validationSettings ?? throw new ArgumentNullException(nameof(validationSettings));
        }

        public IEnumerable<ErrorDetails> Validate(CreateOrderItemRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Order == null)
                throw new ArgumentException("Request Order should not be null", nameof(request));
            if (request.Order.CommencementDate == null)
                throw new ArgumentException("Request Order Commencement Date should not be null");

            if (request.CatalogueItemType == null)
            {
                yield return new ErrorDetails("CatalogueItemTypeValidValue", "CatalogueItemType");
            }
            else
            {
                var errors = ValidateDeliveryDate(request.DeliveryDate, request.Order.CommencementDate.Value, request.CatalogueItemType);
                foreach (var error in errors)
                {
                    yield return error;
                }
            }

            var provisioningType = ProvisioningType.FromName(request.ProvisioningTypeName);
            if (provisioningType == null)
            {
                yield return new ErrorDetails("ProvisioningTypeValidValue", nameof(CreateOrderItemSolutionModel.ProvisioningType));
            }
            else
            {
                var errors = ValidateEstimationPeriod(request.EstimationPeriodName, provisioningType);
                foreach (var error in errors)
                {
                    yield return error;
                }
            }

            var cataloguePriceType = CataloguePriceType.FromName(request.CataloguePriceTypeName);

            if (cataloguePriceType == null)
            {
                yield return new ErrorDetails("TypeValidValue", nameof(CreateOrderItemSolutionModel.Type));
            }
            else if (cataloguePriceType.Equals(CataloguePriceType.Flat))
            {
                if (request.Price == null)
                {
                    yield return new ErrorDetails("PriceRequired", nameof(CreateOrderItemSolutionModel.Price));
                }
            }

            if (request.CurrencyCode != "GBP")
            {
                yield return new ErrorDetails("CurrencyCodeValidValue", nameof(CreateOrderItemSolutionModel.CurrencyCode));
            }
        }

        public IEnumerable<ErrorDetails> Validate(UpdateOrderItemRequest request, CatalogueItemType itemType, ProvisioningType provisioningType)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (itemType == null)
                throw new ArgumentNullException(nameof(itemType));
            if (provisioningType == null)
                throw new ArgumentNullException(nameof(provisioningType));
            if (request.Order == null)
                throw new ArgumentException("Request Order should not be null", nameof(request));
            if (request.Order.CommencementDate == null)
                throw new ArgumentException("Request Order Commencement Date should not be null");

            var errors = new List<ErrorDetails>();

            errors.AddRange(ValidateDeliveryDate(request.DeliveryDate, request.Order.CommencementDate.Value, itemType));
            errors.AddRange(ValidateEstimationPeriod(request.EstimationPeriodName, provisioningType));
            return errors;
        }

        private IEnumerable<ErrorDetails> ValidateDeliveryDate(DateTime? deliveryDate, DateTime commencementDate, CatalogueItemType itemType)
        {
            if (itemType.Equals(CatalogueItemType.Solution))
            {
                if (deliveryDate == null)
                {
                    yield return new ErrorDetails("DeliveryDateRequired", nameof(CreateOrderItemSolutionModel.DeliveryDate));
                }
                else if (deliveryDate.Value > commencementDate.AddDays(_validationSettings.MaxDeliveryDateWeekOffset * 7) 
                      || deliveryDate.Value < commencementDate)
                {
                    yield return new ErrorDetails("DeliveryDateOutsideDeliveryWindow", nameof(CreateOrderItemSolutionModel.DeliveryDate));
                }
            }
        }

        private static IEnumerable<ErrorDetails> ValidateEstimationPeriod(string estimationPeriodName, ProvisioningType provisioningType)
        {
            if (provisioningType.Equals(ProvisioningType.OnDemand))
            {
                if (estimationPeriodName == null)
                {
                    yield return new ErrorDetails("EstimationPeriodRequiredIfVariableOnDemand", nameof(CreateOrderItemSolutionModel.EstimationPeriod));
                }
                else
                {
                    var estimationPeriod = TimeUnit.FromName(estimationPeriodName);
                    if (estimationPeriod == null)
                    {
                        yield return new ErrorDetails("EstimationPeriodValidValue", nameof(CreateOrderItemSolutionModel.EstimationPeriod));
                    }
                }
            }
        }
    }
}
