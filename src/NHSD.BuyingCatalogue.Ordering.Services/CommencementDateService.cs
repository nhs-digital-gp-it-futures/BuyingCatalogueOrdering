using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class CommencementDateService : ICommencementDateService
    {
        private readonly ApplicationDbContext context;

        public CommencementDateService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DateTime?> GetCommencementDate(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Select(o => o.CommencementDate)
                .SingleOrDefaultAsync();
        }

        public async Task SetCommencementDate(Order order, DateTime? commencementDate)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (commencementDate is null)
                throw new ArgumentNullException(nameof(commencementDate));

            order.CommencementDate = commencementDate.Value;
            await context.SaveChangesAsync();
        }
    }
}
