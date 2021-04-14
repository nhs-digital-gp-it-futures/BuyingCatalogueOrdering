using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class OrderDescriptionService : IOrderDescriptionService
    {
        private readonly ApplicationDbContext context;

        public OrderDescriptionService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> GetOrderDescription(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Select(o => o.Description)
                .SingleOrDefaultAsync();
        }

        public async Task SetOrderDescription(Order order, string description)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (description is null)
                throw new ArgumentNullException(nameof(description));

            order.Description = description;
            await context.SaveChangesAsync();
        }
    }
}
