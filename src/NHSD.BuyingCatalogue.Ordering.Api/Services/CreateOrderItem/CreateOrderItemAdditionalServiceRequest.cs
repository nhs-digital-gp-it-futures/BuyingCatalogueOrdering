using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    internal sealed class CreateOrderItemAdditionalServiceRequest : CreateOrderItemWithServiceRecipientRequest
    {
        public CreateOrderItemAdditionalServiceRequest(Order order, CreateOrderItemModel model)
            : base(order, model, CatalogueItemType.AdditionalService)
        {
            CatalogueSolutionId = model.CatalogueSolutionId;
        }
    }
}
