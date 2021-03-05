using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services
{
    internal sealed class ServiceRecipientService : IServiceRecipientService
    {
        private readonly ApplicationDbContext context;

        public ServiceRecipientService(ApplicationDbContext context) =>
            this.context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<IReadOnlyDictionary<string, ServiceRecipient>> AddOrUpdateServiceRecipients(
            IEnumerable<IServiceRecipient> recipients)
        {
            var requestRecipients = recipients
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .ToDictionary(r => r.OdsCode);

            var existingServiceRecipients = await context.ServiceRecipient
                .Where(s => requestRecipients.Keys.Contains(s.OdsCode))
                .ToListAsync();

            foreach (var recipient in existingServiceRecipients)
                recipient.Name = requestRecipients[recipient.OdsCode].Name;

            var newServiceRecipients = existingServiceRecipients.Except(requestRecipients.Values).ToList();

            // ReSharper disable once MethodHasAsyncOverload
            // Non-async method recommended over async version for most cases (see EF Core docs)
            context.ServiceRecipient.AddRange(newServiceRecipients);

            return existingServiceRecipients.Union(newServiceRecipients).ToDictionary(r => r.OdsCode);
        }
    }
}
