using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IOrderDescriptionService
    {
        Task<string> GetOrderDescription(CallOffId callOffId);

        Task SetOrderDescription(Order order, string description);
    }
}
