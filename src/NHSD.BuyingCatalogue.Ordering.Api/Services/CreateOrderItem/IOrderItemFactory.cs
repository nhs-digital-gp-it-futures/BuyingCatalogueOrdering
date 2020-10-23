using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public interface IOrderItemFactory
    {
        OrderItem Create(CreateOrderItemRequest request);
    }
}
