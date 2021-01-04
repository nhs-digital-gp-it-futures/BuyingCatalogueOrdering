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
        private const string DefaultOrderId = "C010000-01";
        private readonly ApplicationDbContext context;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Order>> ListOrdersByOrganisationIdAsync(Guid organisationId)
        {
            return await context.Order
                .Include(o => o.OrganisationAddress)
                .Include(o => o.OrganisationContact)
                .Include(o => o.SupplierAddress)
                .Include(o => o.SupplierContact)
                .Where(o => o.OrganisationId == organisationId)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return null;

            var order = await context.Order.FindAsync(orderId);
            if (order is null)
                return null;

            await context.Entry(order).Reference(o => o.OrganisationAddress).LoadAsync();
            await context.Entry(order).Reference(o => o.OrganisationContact).LoadAsync();
            await context.Entry(order).Reference(o => o.SupplierAddress).LoadAsync();
            await context.Entry(order).Reference(o => o.SupplierContact).LoadAsync();
            await context.Entry(order).Collection(o => o.OrderItems).LoadAsync();
            await context.Entry(order).Collection(o => o.ServiceRecipients).LoadAsync();

            return order;
        }

        public async Task<Order> GetOrderByIdAsync(string orderId, Action<IOrderQuery> configureQuery)
        {
            if (configureQuery is null)
                throw new ArgumentNullException(nameof(configureQuery));

            var query = new OrderQuery(context.Order);
            configureQuery(query);

            return await query.Execute(orderId);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            context.Order.Update(order);
            await context.SaveChangesAsync();
        }

        public async Task<string> CreateOrderAsync(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            await using var dbContextTransaction = await context.Database.BeginTransactionAsync();
            order.OrderId ??= await GetIncrementedOrderId();

            await context.Order.AddAsync(order);
            await context.SaveChangesAsync();
            await dbContextTransaction.CommitAsync();

            return order.OrderId;
        }

        private async Task<string> GetIncrementedOrderId()
        {
            var resultOrderId = DefaultOrderId;
            var latestOrderId = await GetLatestOrderIdByCreationDateAsync();
            if (string.IsNullOrEmpty(latestOrderId))
                return resultOrderId;

            var numberSection = latestOrderId.Substring(1, 6);
            var orderNumber = int.Parse(numberSection, CultureInfo.InvariantCulture);
            resultOrderId = $"C{orderNumber + 1:D6}-01";
            return resultOrderId;
        }

        private async Task<string> GetLatestOrderIdByCreationDateAsync()
        {
            var latestOrder = await context
                .Order
                .IgnoreQueryFilters()
                .OrderByDescending(o => o.Created).FirstOrDefaultAsync();

            return latestOrder?.OrderId;
        }
    }
}
