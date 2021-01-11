using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal sealed class DefaultDeliveryDateValidator : IDefaultDeliveryDateValidator
    {
        internal static readonly ErrorModel CommencementDateRequired = new(nameof(Order.CommencementDate) + "Required");
        internal static readonly ErrorModel OutsideWindow = new(FieldName + "OutsideDeliveryWindow", FieldName);

        private const string FieldName = nameof(DefaultDeliveryDateModel.DeliveryDate);

        private readonly ValidationSettings settings;

        public DefaultDeliveryDateValidator(ValidationSettings settings) =>
            this.settings = settings;

        public (bool IsValid, ErrorsModel Errors) Validate(
            DefaultDeliveryDateModel defaultDeliveryDate,
            DateTime? commencementDate)
        {
            // ReSharper disable once PossibleInvalidOperationException (covered by model validation)
            var deliveryDate = defaultDeliveryDate.DeliveryDate.Value;

            if (!commencementDate.HasValue)
                return Result(CommencementDateRequired);

            if (deliveryDate < commencementDate)
                return Result(OutsideWindow);

            return deliveryDate <= commencementDate.Value.AddDays(settings.MaxDeliveryDateOffsetInDays)
                ? Result()
                : Result(OutsideWindow);
        }

        private static (bool IsValid, ErrorsModel Errors) Result(params ErrorModel[] errors)
        {
            return (errors.Length == 0, new ErrorsModel(errors));
        }
    }
}
