using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Repositories
{
    public sealed class ServiceRecipientRepository : IServiceRecipientRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRecipientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRecipient>> ListServiceRecipientsByOrderIdAsync(string orderId)
        {
            return await _context.ServiceRecipient
                .Include(x => x.Order)
                .Where(s => s.Order.OrderId == orderId).ToListAsync();
        }

        public async Task UpdateServiceRecipientsAsync(Order order, IEnumerable<ServiceRecipient> recipientsUpdates)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (recipientsUpdates == null)
            {
                throw new ArgumentNullException(nameof(recipientsUpdates));
            }

            var serviceRecipientsToAdd = recipientsUpdates.ToList();
            var serviceRecipientsToRemove = (await ListServiceRecipientsByOrderIdAsync(order.OrderId)).ToList();

            var noChangeServiceRecipients = serviceRecipientsToRemove.Select(s => s.OdsCode)
                    .Intersect(serviceRecipientsToAdd.Select(s => s.OdsCode))
                    .ToList();

            serviceRecipientsToRemove.RemoveAll(s => noChangeServiceRecipients.Contains(s.OdsCode));
            serviceRecipientsToAdd.RemoveAll(s => noChangeServiceRecipients.Contains(s.OdsCode));

            _context.ServiceRecipient.RemoveRange(serviceRecipientsToRemove);
            _context.ServiceRecipient.AddRange(serviceRecipientsToAdd);

            await _context.SaveChangesAsync();
        }
    }
}
