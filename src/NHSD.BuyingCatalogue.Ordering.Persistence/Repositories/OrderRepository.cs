using System;
using System.Collections.Generic;
using System.Globalization;
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
        public const string DefaultOrderId = "C000000-01";
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

        public async Task<string> GetLatestOrderIdByCreationDate()
        {
            var latestOrder = await _context.Order.OrderByDescending(o => o.Created).FirstOrDefaultAsync();
            return latestOrder?.OrderId;
        }

        public async Task<string> CreateOrderAsync(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            using (var dbContextTransaction = _context.Database.BeginTransactionAsync())
            {
                if (order.OrderId == null)
                {
                    order.OrderId = await GetIncrementedOrderId();
                }

                order.OrderStatus = await _context.OrderStatus.FindAsync(order.OrderStatus.OrderStatusId) ?? order.OrderStatus;

                _context.Order.Add(order);
                await _context.SaveChangesAsync();
                await (await dbContextTransaction).CommitAsync();
            }

            return order.OrderId;
        }

        private async Task<string> GetIncrementedOrderId()
        {
            var resultOrderId = DefaultOrderId;
            var latestOrderId = await GetLatestOrderIdByCreationDate();
            if (!string.IsNullOrEmpty(latestOrderId))
            {
                var numberSection = latestOrderId.Substring(1, 6);
                var orderNumber = int.Parse(numberSection, CultureInfo.InvariantCulture);
                resultOrderId = $"C{orderNumber + 1:D6}-01";
            }
            return resultOrderId;
        }
    }
}
