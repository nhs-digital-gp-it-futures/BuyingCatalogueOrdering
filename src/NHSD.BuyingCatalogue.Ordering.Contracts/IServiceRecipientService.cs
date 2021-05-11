using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IServiceRecipientService
    {
        Task<List<ServiceRecipient>> GetAllOrderItemRecipients(CallOffId callOffId);

        Task<IReadOnlyDictionary<string, ServiceRecipient>> AddOrUpdateServiceRecipients(
            IEnumerable<ServiceRecipient> recipients);
    }
}
