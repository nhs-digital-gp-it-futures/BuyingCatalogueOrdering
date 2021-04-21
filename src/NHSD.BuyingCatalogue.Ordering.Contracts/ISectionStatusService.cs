using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface ISectionStatusService
    {
        Task<Order> GetOrder(CallOffId callOffId);

        Task SetSectionStatus(Order order, string sectionId);
    }
}
