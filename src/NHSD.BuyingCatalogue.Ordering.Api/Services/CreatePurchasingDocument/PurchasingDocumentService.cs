using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public sealed class PurchasingDocumentService : IPurchasingDocumentService
    {
        public async Task CreateDocumentAsync(Stream stream, Order order)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var records = new List<PurchaseOrderItem>
            {
                new PurchaseOrderItem{ CallOffPartyId = order.OrderId }
            };

            await using var streamWriter = new StreamWriter(stream, leaveOpen:true);
            await using var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);

            csvWriter.Configuration.RegisterClassMap<PurchaseDocumentSettingsMap>();
            await csvWriter.WriteRecordsAsync(records);
        }

        private sealed class PurchaseDocumentSettingsMap : ClassMap<PurchaseOrderItem>
        {
            public PurchaseDocumentSettingsMap()
            {
                Map(x => x.CallOffPartyId).Index(0).Name("Call off Party Id");
            }
        }
    }
}
