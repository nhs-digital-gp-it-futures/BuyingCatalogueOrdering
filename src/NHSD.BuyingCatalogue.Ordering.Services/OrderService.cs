using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class OrderService : IOrderService
    {
        private readonly ApplicationDbContext context;

        public OrderService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> GetOrder(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty).ThenInclude(p => p.Address)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier).ThenInclude(s => s.Address)
                .Include(o => o.SupplierContact)
                .Include(o => o.ServiceInstanceItems).Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(o => o.OrderItems).ThenInclude(i => i.PricingUnit)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public async Task<IList<Order>> GetOrders(Guid organisationId)
        {
            return await context.OrderingParty
                .Where(o => o.Id == organisationId)
                .SelectMany(o => o.Orders)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Order> GetOrderSummary(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.SupplierContact)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients)
                .Include(o => o.Progress)
                .AsQueryable()
                .SingleOrDefaultAsync();
        }

        public async Task<Order> GetOrderForStatusUpdate(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty)
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(o => o.OrderItems).ThenInclude(i => i.PricingUnit)
                .Include(o => o.Progress)
                .SingleOrDefaultAsync();
        }

        public async Task<Order> CreateOrder(string description, Guid organisationId)
        {
            OrderingParty orderingParty = (await GetOrderingParty(organisationId)) ?? new OrderingParty { Id = organisationId };

            var order = new Order
            {
                Description = description,
                OrderingParty = orderingParty,
            };

            context.Add(order);
            await context.SaveChangesAsync();

            return order;
        }

        public async Task DeleteOrder(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            order.IsDeleted = true;

            await context.SaveChangesAsync();
        }

        private async Task<OrderingParty> GetOrderingParty(Guid organisationId)
        {
            return await context.OrderingParty.FindAsync(organisationId);
        }
    }
}
