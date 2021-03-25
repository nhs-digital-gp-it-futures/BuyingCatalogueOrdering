using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IOrderService
    {
        Task<DateTime?> GetCommencementDate(CallOffId callOffId);

        Task SetCommencementDate(Order order, DateTime? commencementDate);
    }
}
