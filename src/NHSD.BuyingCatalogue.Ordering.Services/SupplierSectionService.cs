using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class SupplierSectionService : ISupplierSectionService
    {
        private readonly ApplicationDbContext context;

        public SupplierSectionService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> GetOrder(CallOffId callOffId)
        {
            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.Supplier).ThenInclude(s => s.Address)
                .Include(o => o.SupplierContact)
                .SingleOrDefaultAsync();
        }

        public async Task SetSupplierSection(Order order, Supplier supplier, Contact contact)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            if (contact is null)
                throw new ArgumentNullException(nameof(contact));

            order.Supplier ??= await context.Supplier.FindAsync(supplier.Id) ?? new Supplier
            {
                Id = supplier.Id,
            };

            order.Supplier.Name = supplier.Name;
            order.Supplier.Address = supplier.Address;
            order.SupplierContact = contact;

            await context.SaveChangesAsync();
        }
    }
}
