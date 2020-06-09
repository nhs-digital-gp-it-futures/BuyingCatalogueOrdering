using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Application.Persistence
{
    public interface IServiceRecipientRepository
    {
        Task<IEnumerable<ServiceRecipient>> ListServiceRecipientsByOrderIdAsync(string orderId);
        Task UpdateAsync(string orderId, IEnumerable<ServiceRecipient> recipientsUpdates);
    }
}

