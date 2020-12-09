using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public interface ICreateOrderItemValidator
    {
        ValidationResult Validate(CreateOrderItemRequest request);

        AggregateValidationResult Validate(
            IReadOnlyList<CreateOrderItemRequest> requests,
            IEnumerable<OrderItem> existingOrderItems);
    }
}
