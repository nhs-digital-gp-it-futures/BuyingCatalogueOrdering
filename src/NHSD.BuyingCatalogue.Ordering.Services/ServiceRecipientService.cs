using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class ServiceRecipientService : IServiceRecipientService
    {
        private readonly ApplicationDbContext context;

        public ServiceRecipientService(ApplicationDbContext context) =>
            this.context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<List<ServiceRecipient>> GetAllOrderItemRecipients(CallOffId callOffId)
        {
            if (!await context.Order.AnyAsync(o => o.Id == callOffId.Id))
                return null;

            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .SelectMany(o => o.OrderItems)
                .Where(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution)
                .SelectMany(o => o.OrderItemRecipients)
                .Select(r => new { r.Recipient.OdsCode, r.Recipient.Name })
                .Distinct()
                .OrderBy(r => r.Name)
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyDictionary<string, ServiceRecipient>> AddOrUpdateServiceRecipients(
            IEnumerable<ServiceRecipient> recipients)
        {
            var requestRecipients = recipients.ToDictionary(r => r.OdsCode);

            var existingServiceRecipients = await context.ServiceRecipient
                .Where(s => requestRecipients.Keys.Contains(s.OdsCode))
                .ToListAsync();

            foreach (var recipient in existingServiceRecipients)
                recipient.Name = requestRecipients[recipient.OdsCode].Name;

            var newServiceRecipients = requestRecipients.Values.Except(existingServiceRecipients).ToList();

            // ReSharper disable once MethodHasAsyncOverload
            // Non-async method recommended over async version for most cases (see EF Core docs)
            context.ServiceRecipient.AddRange(newServiceRecipients);

            return existingServiceRecipients.Union(newServiceRecipients).ToDictionary(r => r.OdsCode);
        }
    }
}
