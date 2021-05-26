using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class OrderItemService : IOrderItemService
    {
        private readonly ApplicationDbContext context;

        public OrderItemService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> GetOrder(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.DefaultDeliveryDates)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients)
                .Include(o => o.Progress)
                .SingleOrDefaultAsync();
        }

        public async Task<Order> GetOrderWithCatalogueItems(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.Progress)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.CatalogueItem)
                .SingleOrDefaultAsync();
        }

        public async Task<OrderItem> GetOrderItem(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = o =>
                o.OrderItems.Where(i => i.CatalogueItem.Id == catalogueItemId);

            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.PricingUnit)
                .SelectMany(orderItems)
                .SingleOrDefaultAsync();
        }

        public async Task<List<OrderItem>> GetOrderItems(CallOffId callOffId, CatalogueItemType? catalogueItemType)
        {
            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = catalogueItemType is null
                ? o => o.OrderItems
                : o => o.OrderItems.Where(i => i.CatalogueItem.CatalogueItemType == catalogueItemType.Value);

            if (!await context.Order.AnyAsync(o => o.Id == callOffId.Id))
                return null;

            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.PricingUnit)
                .SelectMany(orderItems)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> DeleteOrderItem(Order order, CatalogueItemId catalogueItemId)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var result = order.DeleteOrderItemAndUpdateProgress(catalogueItemId);

            await context.SaveChangesAsync();

            return result;
        }
    }
}
