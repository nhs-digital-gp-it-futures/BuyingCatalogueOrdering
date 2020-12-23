using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Repositories
{
    public sealed class OrderQuery : IOrderQuery
    {
        private IQueryable<Order> query;
        private bool trackChanges = true;

        internal OrderQuery(IQueryable<Order> query) => this.query = query;

        public IOrderQuery WithoutTracking()
        {
            trackChanges = false;
            return this;
        }

        public IOrderQuery WithOrderItems() => Include(o => o.OrderItems);

        public IOrderQuery WithOrganisationDetails() => Include(o => o.OrganisationAddress).Include(o => o.OrganisationContact);

        public IOrderQuery WithServiceInstanceItems() => Include(o => o.ServiceInstanceItems);

        public IOrderQuery WithServiceRecipients() => Include(o => o.ServiceRecipients);

        public IOrderQuery WithSupplierDetails() => Include(o => o.SupplierAddress).Include(o => o.SupplierContact);

        internal async Task<Order> Execute(string orderId)
        {
            if (orderId is null)
                return null;

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        private OrderQuery Include<TProperty>(Expression<Func<Order, TProperty>> navigationPropertyPath)
        {
            query = query.Include(navigationPropertyPath);
            return this;
        }
    }
}
