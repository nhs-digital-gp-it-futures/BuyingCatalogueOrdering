using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    internal abstract class CreateOrderItemWithServiceRecipientRequest : CreateOrderItemRequest
    {
        protected CreateOrderItemWithServiceRecipientRequest(
            Order order,
            CreateOrderItemModel model,
            CatalogueItemType itemType)
            : base(order, model, itemType)
        {
            var serviceRecipient = model.ServiceRecipient;
            var odsCode = serviceRecipient?.OdsCode;

            OdsCode = odsCode;
            PriceTimeUnit = model.TimeUnit?.ToTimeUnit();

            if (serviceRecipient is null)
                return;

            ServiceRecipient = new ServiceRecipient(
                order.OrderId,
                odsCode,
                serviceRecipient.Name);
        }
    }
}
