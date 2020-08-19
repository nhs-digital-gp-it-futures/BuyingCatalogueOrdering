using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.DocumentService
{
    public sealed class PurchasingDocumentService : IPurchasingDocumentService
    {
        public async Task Create(Stream stream, Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var records = new List<PurchaseDocumentSettings>
            {
                new PurchaseDocumentSettings{ CallOffPartyId = order.OrderId }
            };

            await using var streamWriter = new StreamWriter(stream, leaveOpen:true);
            await using var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);

            csvWriter.Configuration.RegisterClassMap<PurchaseDocumentSettingsMap>();
            await csvWriter.WriteRecordsAsync(records);
        }
    }
}
