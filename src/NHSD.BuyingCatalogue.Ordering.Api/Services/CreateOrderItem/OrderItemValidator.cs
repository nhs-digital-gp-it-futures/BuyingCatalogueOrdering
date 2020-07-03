using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public class OrderItemValidator : ICreateOrderItemValidator, IUpdateOrderItemValidator
    {
        public static readonly IEnumerable<string> ValidEstimationPeriodNames = new List<string> {"MONTH", "YEAR"};

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

            var errors = new List<ErrorDetails>();
            
            if (request.CatalogueItemType == null)
            {
                errors.Add(new ErrorDetails("TypeValidValue", "Type"));
            }
            else
            {
                errors.AddRange(ValidateDeliveryDate(request.DeliveryDate, request.Order.CommencementDate.Value, request.CatalogueItemType));
            }

            var provisioningType = ProvisioningType.FromName(request.ProvisioningTypeName);
            if (provisioningType == null)
            {
                errors.Add(new ErrorDetails("ProvisioningTypeValidValue", "ProvisioningType"));
            }
            else
            {
                errors.AddRange(ValidateEstimationPeriod(request.EstimationPeriodName, provisioningType));
            }

            var cataloguePriceType = CataloguePriceType.FromName(request.CataloguePriceTypeName);

            if (cataloguePriceType == null)
            {
                errors.Add(new ErrorDetails("TypeValidValue", "Type"));
            }
            else if (cataloguePriceType.Equals(CataloguePriceType.Flat))
            {
                if (request.Price == null)
                {
                    errors.Add(new ErrorDetails("PriceRequired", "Price"));
                }
            }

            if (request.CurrencyCode != "GBP")
            {
                errors.Add(new ErrorDetails("CurrencyCodeValidValue", "CurrencyCode"));
            }

            return errors;
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
            var errors = new List<ErrorDetails>();

            if (itemType.Equals(CatalogueItemType.Solution))
            {
                if (deliveryDate == null)
                {
                    errors.Add(new ErrorDetails("DeliveryDateRequired", "DeliveryDate"));
                }
                else if (deliveryDate.Value > commencementDate.AddDays(_validationSettings.MaxDeliveryDateMonthOffset * 7) 
                      || deliveryDate.Value < commencementDate)
                {
                    errors.Add(new ErrorDetails("DeliveryDateOutsideDeliveryWindow", "DeliveryDate"));
                }
            }

            return errors;
        }

        private static IEnumerable<ErrorDetails> ValidateEstimationPeriod(string estimationPeriodName, ProvisioningType provisioningType)
        {
            var errors = new List<ErrorDetails>();
            if (provisioningType.Equals(ProvisioningType.OnDemand))
            {
                if (estimationPeriodName == null)
                {
                    errors.Add(new ErrorDetails("EstimationPeriodRequiredIfVariableOnDemand", "EstimationPeriod"));
                }
                else if (!ValidEstimationPeriodNames.Contains(estimationPeriodName.ToUpperInvariant()))
                {
                    errors.Add(new ErrorDetails("EstimationPeriodValidValue", "EstimationPeriod"));
                }
            }
            return errors;
        }
    }
}
