using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IDefaultDeliveryDateService
    {
        Task<DateTime?> GetDefaultDeliveryDate(CallOffId callOffId, CatalogueItemId catalogueItemId);

        Task<DeliveryDateResult> SetDefaultDeliveryDate(CallOffId callOffId, CatalogueItemId catalogueItemId, DateTime? deliveryDate);

        Task<Order> GetDefaultDeliveryOrder(CallOffId callOffId, CatalogueItemId catalogueItemId);
    }
}
