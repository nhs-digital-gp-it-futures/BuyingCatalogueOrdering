using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public interface IUpdateOrderItemValidator
    {
        IEnumerable<ErrorDetails> Validate(UpdateOrderItemRequest request, CatalogueItemType itemType, ProvisioningType? provisioningType);
    }
}
