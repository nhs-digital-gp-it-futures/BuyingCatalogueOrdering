using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IFundingSourceService
    {
        Task<bool?> GetFundingSource(CallOffId callOffId);

        Task SetFundingSource(Order order, bool? onlyGms);
    }
}
