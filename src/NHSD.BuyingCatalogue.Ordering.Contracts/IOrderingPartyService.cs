using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IOrderingPartyService
    {
        Task<Order> GetOrder(CallOffId callOffId);

        Task SetOrderingParty(Order order, OrderingParty orderingParty, Contact contact);
    }
}
