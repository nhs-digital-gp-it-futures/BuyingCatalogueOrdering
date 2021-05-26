using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IOrderItemService
    {
        Task<Order> GetOrder(CallOffId callOffId);

        Task<Order> GetOrderWithCatalogueItems(CallOffId callOffId);

        Task<OrderItem> GetOrderItem(CallOffId callOffId, CatalogueItemId catalogueItemId);

        Task<List<OrderItem>> GetOrderItems(CallOffId callOffId, CatalogueItemType? catalogueItemType);

        Task<int> DeleteOrderItem(Order order, CatalogueItemId catalogueItemId);
    }
}
