using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IOrderService
    {
        Task<Order> GetOrder(CallOffId callOffId);

        Task<IList<Order>> GetOrderList(Guid organisationId);

        Task<Order> GetOrderSummary(CallOffId callOffId);

        Task<Order> GetOrderCompletedStatus(CallOffId callOffId);

        Task<OrderingParty> GetOrderingParty(Guid organisationId);

        Task<Order> CreateOrder(string description, Guid organisationId);

        Task DeleteOrder(Order order);
    }
}
