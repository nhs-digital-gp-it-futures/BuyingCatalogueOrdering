using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> ListOrdersByOrganisationIdAsync(Guid organisationId)
        {
            return await _context.Order.Include(x => x.OrderStatus).Where(o => o.OrganisationId == organisationId)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _context.Order.FindAsync(orderId);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            _context.Order.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetLatestOrderByCreationDate()
        {
            var latestOrder = await _context.Order.Include(x => x.OrderStatus).OrderByDescending(o => o.Created).FirstOrDefaultAsync();
            return latestOrder?.OrderId;
        }

        public async Task CreateOrderAsync(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            _context.Order.Add(order);
            await _context.SaveChangesAsync();
        }
    }
}
