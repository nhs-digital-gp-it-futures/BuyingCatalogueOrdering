using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class FundingSourceService : IFundingSourceService
    {
        private readonly ApplicationDbContext context;

        public FundingSourceService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool?> GetFundingSource(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Select(o => o.FundingSourceOnlyGms)
                .SingleOrDefaultAsync();
        }

        public async Task SetFundingSource(Order order, bool? onlyGms)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (onlyGms is null)
                throw new ArgumentNullException(nameof(onlyGms));

            order.FundingSourceOnlyGms = onlyGms;
            await context.SaveChangesAsync();
        }
    }
}
