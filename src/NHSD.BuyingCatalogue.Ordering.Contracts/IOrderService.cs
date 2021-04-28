using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IOrderService
    {
        Task<Order> GetOrder(CallOffId callOffId);

        Task<IList<Order>> GetOrders(Guid organisationId);

        Task<Order> GetOrderSummary(CallOffId callOffId);

        Task<Order> GetOrderForStatusUpdate(CallOffId callOffId);

        Task<Order> CreateOrder(string description, Guid organisationId);

        Task DeleteOrder(Order order);
    }
}
