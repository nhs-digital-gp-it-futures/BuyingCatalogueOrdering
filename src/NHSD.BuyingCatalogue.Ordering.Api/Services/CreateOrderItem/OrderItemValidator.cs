using System;
using System.Collections.Generic;
using EnumsNET;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
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

        public ValidationResult Validate(CreateOrderItemRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var commencementDate = request.Order.CommencementDate;

            if (commencementDate is null)
                throw OrderCommencementDateArgumentException(nameof(request));

            return new ValidationResult(ValidateDeliveryDate(request.DeliveryDate, commencementDate.Value, request.CatalogueItemType));
        }

        public IEnumerable<ErrorDetails> Validate(UpdateOrderItemRequest request, CatalogueItemType itemType, ProvisioningType? provisioningType)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (provisioningType is null)
                throw new ArgumentNullException(nameof(provisioningType));

            if (request.Order.CommencementDate is null)
                throw OrderCommencementDateArgumentException(nameof(request));

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
                yield return EstimationPeriodError("RequiredIfVariableOnDemand");
            }
            else if (!estimationPeriod.Value.IsDefined())
            {
                yield return EstimationPeriodError("ValidValue");
            }
        }

        private static ErrorDetails DeliveryDateError(string error)
        {
            return ErrorDetails(nameof(CreateOrderItemModel.DeliveryDate), error);
        }

        private static ErrorDetails EstimationPeriodError(string error)
        {
            return ErrorDetails(nameof(UpdateOrderItemModel.EstimationPeriod), error);
        }

        private static ErrorDetails ErrorDetails(string propertyName, string error)
        {
            return new ErrorDetails(propertyName + error, propertyName);
        }

        private static IReadOnlyList<ErrorDetails> NoErrors() => Array.Empty<ErrorDetails>();

        private static Exception OrderCommencementDateArgumentException(string paramName)
        {
            return new ArgumentException($"{OrderProperty(paramName)}.{nameof(Order.CommencementDate)} should not be null.", paramName);
        }

        private static string OrderProperty(string paramName) => $"{paramName}.{nameof(CreateOrderItemRequest.Order)}";

        private static IReadOnlyList<ErrorDetails> Errors(params ErrorDetails[] details) => details;

        private IReadOnlyList<ErrorDetails> ValidateDeliveryDate(DateTime? deliveryDate, DateTime commencementDate, CatalogueItemType itemType)
        {
            if (!itemType.Equals(CatalogueItemType.Solution))
                return NoErrors();

            if (deliveryDate is null)
                return Errors(DeliveryDateError("Required"));

            return IsDeliveryDateWithinWindow(deliveryDate.Value, commencementDate)
                ? NoErrors()
                : Errors(DeliveryDateError("OutsideDeliveryWindow"));
        }

        private bool IsDeliveryDateWithinWindow(DateTime deliveryDate, DateTime commencementDate)
        {
            return deliveryDate >= commencementDate
                   && deliveryDate <= commencementDate.AddDays(validationSettings.MaxDeliveryDateOffsetInDays);
        }
    }
}
