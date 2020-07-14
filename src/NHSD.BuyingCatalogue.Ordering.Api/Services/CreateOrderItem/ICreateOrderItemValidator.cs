using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public interface ICreateOrderItemValidator
    {
        IEnumerable<ErrorDetails> Validate(CreateOrderItemRequest request);
    }
}
