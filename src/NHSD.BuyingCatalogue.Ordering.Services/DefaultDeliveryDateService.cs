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
    public class DefaultDeliveryDateService : IDefaultDeliveryDateService
    {
        private readonly ApplicationDbContext context;

        public DefaultDeliveryDateService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DateTime?> GetDefaultDeliveryDate(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            Expression<Func<Order, IEnumerable<DefaultDeliveryDate>>> defaultDeliveryDate = o
                => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catalogueItemId);

            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(defaultDeliveryDate)
                .SelectMany(defaultDeliveryDate)
                .Select(d => d.DeliveryDate)
                .SingleOrDefaultAsync();
        }

        public async Task<DeliveryDateResult> SetDefaultDeliveryDate(CallOffId callOffId, CatalogueItemId catalogueItemId, DateTime? deliveryDate)
        {
          var order = await GetDefaultDeliveryOrder(callOffId, catalogueItemId);

          // ReSharper disable once PossibleInvalidOperationException (covered by model validation)
          DeliveryDateResult addedOrUpdated = order.SetDefaultDeliveryDate(catalogueItemId, deliveryDate.Value);

          await context.SaveChangesAsync();

          return addedOrUpdated;
        }

        public async Task<Order> GetDefaultDeliveryOrder(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catalogueItemId))
                .SingleOrDefaultAsync();
        }
    }
}
