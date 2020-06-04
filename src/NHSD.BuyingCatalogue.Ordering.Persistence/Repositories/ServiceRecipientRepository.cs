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

        public async Task<IEnumerable<ServiceRecipient>> ListServiceRecipientsByOrderId(string orderId)
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
            var serviceRecipientsToRemove = (await ListServiceRecipientsByOrderId(order.OrderId)).ToList();

            var noChangeServiceRecipients = serviceRecipientsToRemove.Select(s => s.OdsCode).Intersect(serviceRecipientsToAdd.Select(s => s.OdsCode));
            serviceRecipientsToRemove.RemoveAll(s => noChangeServiceRecipients.Contains(s.OdsCode));
            serviceRecipientsToAdd.RemoveAll(s => noChangeServiceRecipients.Contains(s.OdsCode));

            foreach (ServiceRecipient recipient in serviceRecipientsToRemove)
            {
                _context.ServiceRecipient.Remove(recipient);
            }

            foreach (ServiceRecipient recipient in serviceRecipientsToAdd)
            {
                _context.ServiceRecipient.Add(recipient);
            }

            await _context.SaveChangesAsync();
        }
    }
}
