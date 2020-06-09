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

        public async Task<int> GetCountByOrderIdAsync(string orderId)
        {
            if (orderId is null)
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            return await _context.ServiceRecipient
                .Where(x => x.Order.OrderId == orderId)
                .CountAsync();
        }
        

        public async Task UpdateAsync(string orderId, IEnumerable<ServiceRecipient> recipientsUpdates)
        {
            if (recipientsUpdates == null)
            {
                throw new ArgumentNullException(nameof(recipientsUpdates));
            }

            var updateServiceRecipients = recipientsUpdates.ToList();
            var existingServiceRecipients = (await ListServiceRecipientsByOrderIdAsync(orderId)).ToList();

            if (!updateServiceRecipients.Any())
            {
                _context.ServiceRecipient.RemoveRange(existingServiceRecipients);
            }
            else
            {
                var noChangeServiceRecipients = existingServiceRecipients.Intersect(updateServiceRecipients).ToList();

                var existingServiceRecipientsToRemove = existingServiceRecipients.Except(noChangeServiceRecipients);
                var updateServiceRecipientsToAdd = updateServiceRecipients.Except(noChangeServiceRecipients);

                _context.ServiceRecipient.RemoveRange(existingServiceRecipientsToRemove);
                _context.ServiceRecipient.AddRange(updateServiceRecipientsToAdd);
            }
            await _context.SaveChangesAsync();
        }
    }
}
