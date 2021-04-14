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
    public sealed class DefaultDeliveryDateService : IDefaultDeliveryDateService
    {
        private readonly ApplicationDbContext context;

        public DefaultDeliveryDateService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DateTime?> GetDefaultDeliveryDate(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            Expression<Func<Order, IEnumerable<DefaultDeliveryDate>>> defaultDeliveryDateExpression = o
                => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catalogueItemId);

            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(defaultDeliveryDateExpression)
                .SelectMany(defaultDeliveryDateExpression)
                .Select(d => d.DeliveryDate)
                .SingleOrDefaultAsync();
        }

        public async Task<DeliveryDateResult> SetDefaultDeliveryDate(CallOffId callOffId, CatalogueItemId catalogueItemId, DateTime deliveryDate)
        {
          var order = await GetOrder(callOffId, catalogueItemId);

          DeliveryDateResult addedOrUpdated = order.SetDefaultDeliveryDate(catalogueItemId, deliveryDate);

          await context.SaveChangesAsync();

          return addedOrUpdated;
        }

        public async Task<Order> GetOrder(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catalogueItemId))
                .SingleOrDefaultAsync();
        }
    }
}
