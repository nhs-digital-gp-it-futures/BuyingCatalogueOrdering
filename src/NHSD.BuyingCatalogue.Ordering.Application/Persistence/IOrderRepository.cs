using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Application.Persistence
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> ListOrdersByOrganisationIdAsync(Guid organisationId);
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<string> GetLatestOrderIdByCreationDate();
        Task UpdateOrderAsync(Order order);
        Task<string> CreateOrderAsync(Order order);
    }
}
