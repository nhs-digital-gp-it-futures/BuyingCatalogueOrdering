using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    internal sealed class CreateOrderItemSolutionRequest : CreateOrderItemWithServiceRecipientRequest
    {
        public CreateOrderItemSolutionRequest(Order order, CreateOrderItemModel model)
            : base(order, model, CatalogueItemType.Solution)
        {
            DeliveryDate = model.DeliveryDate;
        }
    }
}
