using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IServiceRecipientService
    {
        Task<Order> GetOrder(CallOffId callOffId);

        Task SetOrder(Order order, IReadOnlyList<SelectedServiceRecipient> selectedRecipients);

        Task<List<ServiceRecipient>> GetAllOrderItemRecipient(CallOffId callOffId);

        Task<IReadOnlyDictionary<string, ServiceRecipient>> AddOrUpdateServiceRecipients(
            IEnumerable<ServiceRecipient> recipients);
    }
}
