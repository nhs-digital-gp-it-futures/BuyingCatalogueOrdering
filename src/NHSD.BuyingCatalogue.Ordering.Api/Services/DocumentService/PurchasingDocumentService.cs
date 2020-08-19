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
        public async Task Create(Order order, Stream stream)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var records = new List<PurchaseDocumentSettings>
            {
                new PurchaseDocumentSettings{ CallOffPartyId = order.OrderId }
            };

            await using var writer = new StreamWriter(stream);
            await using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
            csv.Configuration.RegisterClassMap<PurchaseDocumentSettingsMap>();
            await csv.WriteRecordsAsync(records);
            //await csv.FlushAsync();
        }
    }
}
