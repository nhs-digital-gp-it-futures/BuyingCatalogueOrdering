using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface ICommencementDateService
    {
        Task<DateTime?> GetCommencementDate(CallOffId callOffId);

        Task SetCommencementDate(Order order, DateTime? commencementDate);
    }
}
