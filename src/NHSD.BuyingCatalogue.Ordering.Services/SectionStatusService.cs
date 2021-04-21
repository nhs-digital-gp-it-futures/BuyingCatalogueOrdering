using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class SectionStatusService : ISectionStatusService
    {
        private readonly ApplicationDbContext context;

        public SectionStatusService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> GetOrder(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.Progress)
                .SingleOrDefaultAsync();
        }

        public async Task SetSectionStatus(Order order, string sectionId)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (sectionId is null)
                throw new ArgumentNullException(nameof(sectionId));

            switch (sectionId)
            {
                case "catalogue-solutions":
                    order.Progress.CatalogueSolutionsViewed = true;
                    break;
                case "additional-services":
                    order.Progress.AdditionalServicesViewed = true;
                    break;
                case "associated-services":
                    order.Progress.AssociatedServicesViewed = true;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            await context.SaveChangesAsync();
        }
    }
}
