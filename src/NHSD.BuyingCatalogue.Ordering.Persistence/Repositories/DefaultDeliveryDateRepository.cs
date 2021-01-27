using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Repositories
{
    public sealed class DefaultDeliveryDateRepository : IDefaultDeliveryDateRepository
    {
        private readonly ApplicationDbContext context;

        public DefaultDeliveryDateRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> AddOrUpdateAsync(DefaultDeliveryDate defaultDeliveryDate)
        {
            if (defaultDeliveryDate is null)
                throw new ArgumentNullException(nameof(defaultDeliveryDate));

            var exists = await context.DefaultDeliveryDate.AnyAsync(d => d.Equals(defaultDeliveryDate));

            await context.Upsert(defaultDeliveryDate).RunAsync();

            return !exists;
        }

        public async Task<DefaultDeliveryDate> GetAsync(string orderId, string catalogueItemId, int priceId)
        {
            return await context.DefaultDeliveryDate.FindAsync(orderId, catalogueItemId, priceId);
        }
    }
}
