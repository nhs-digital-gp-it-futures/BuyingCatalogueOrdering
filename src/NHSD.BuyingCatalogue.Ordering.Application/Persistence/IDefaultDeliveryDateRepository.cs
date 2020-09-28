using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Application.Persistence
{
    public interface IDefaultDeliveryDateRepository
    {
        Task<bool> AddOrUpdateAsync(DefaultDeliveryDate defaultDeliveryDate);

        Task<DefaultDeliveryDate> GetAsync(string orderId, string catalogueItemId, int priceId);
    }
}
