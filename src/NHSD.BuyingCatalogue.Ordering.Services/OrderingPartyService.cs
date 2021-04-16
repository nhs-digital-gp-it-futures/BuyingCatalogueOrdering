using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class OrderingPartyService : IOrderingPartyService
    {
        private readonly ApplicationDbContext context;

        public OrderingPartyService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> GetOrder(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty).ThenInclude(p => p.Address)
                .Include(o => o.OrderingPartyContact)
                .SingleOrDefaultAsync();
        }

        public async Task SetOrderingParty(Order order, OrderingParty orderingParty, Contact contact)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (orderingParty is null)
                throw new ArgumentNullException(nameof(order));

            if (contact is null)
                throw new ArgumentNullException(nameof(contact));

            order.OrderingParty.Name = orderingParty.Name;
            order.OrderingParty.OdsCode = orderingParty.OdsCode;
            order.OrderingParty.Address = orderingParty.Address;
            order.OrderingPartyContact = contact;

            await context.SaveChangesAsync();
        }
    }
}
