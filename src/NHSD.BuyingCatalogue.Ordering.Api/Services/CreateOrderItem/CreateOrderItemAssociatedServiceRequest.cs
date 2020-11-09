using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    internal sealed class CreateOrderItemAssociatedServiceRequest : CreateOrderItemRequest
    {
        public CreateOrderItemAssociatedServiceRequest(Order order, CreateOrderItemModel model)
            : base(order, model, CatalogueItemType.AssociatedService)
        {
            OdsCode = order.OrganisationOdsCode;
        }
    }
}
