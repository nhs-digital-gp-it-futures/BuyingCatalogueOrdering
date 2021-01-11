using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    public interface IDefaultDeliveryDateValidator
    {
        (bool IsValid, ErrorsModel Errors) Validate(
            DefaultDeliveryDateModel defaultDeliveryDate,
            DateTime? commencementDate);
    }
}
