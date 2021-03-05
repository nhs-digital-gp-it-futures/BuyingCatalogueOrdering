using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Application;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public sealed class CreatePurchasingDocumentService : ICreatePurchasingDocumentService
    {
        private readonly ICsvStreamWriter<OdooOrderItem> csvWriter;
        private readonly ICsvStreamWriter<OdooPatientNumbersOrderItem> patientNumbersCsvWriter;

        public CreatePurchasingDocumentService(
            ICsvStreamWriter<OdooPatientNumbersOrderItem> patientNumbersCsvWriter,
            ICsvStreamWriter<OdooOrderItem> csvWriter)
        {
            this.patientNumbersCsvWriter = patientNumbersCsvWriter ?? throw new ArgumentNullException(nameof(patientNumbersCsvWriter));
            this.csvWriter = csvWriter ?? throw new ArgumentNullException(nameof(csvWriter));
        }

        public async Task CreatePatientNumbersCsvAsync(Stream stream, Order order)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var patientNumbersPriceTypes = order
                .FlattenOrderItems()
                .Select(orderItem => new OdooPatientNumbersOrderItem
                {
                    CallOffAgreementId = order.CallOffId.ToString(),
                    CallOffOrderingPartyId = order.OrderingParty.OdsCode,
                    CallOffOrderingPartyName = order.OrderingParty.Name,
                    CallOffCommencementDate = order.CommencementDate,
                    ServiceRecipientId = orderItem.Recipient.OdsCode,
                    ServiceRecipientName = orderItem.Recipient.Name,
                    ServiceRecipientItemId = $"{order.CallOffId}-{orderItem.Recipient.OdsCode}-{orderItem.ItemId}",
                    SupplierId = order.Supplier.Id,
                    SupplierName = order.Supplier.Name,
                    ProductId = orderItem.CatalogueItem.Id.ToString(),
                    ProductName = orderItem.CatalogueItem.Name,
                    ProductType = orderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                    QuantityOrdered = orderItem.Quantity,
                    UnitOfOrder = orderItem.PricingUnit.Description,
                    Price = orderItem.Price.GetValueOrDefault(),
                    M1Planned = orderItem.DeliveryDate,
                });

            await patientNumbersCsvWriter.WriteRecordsAsync(stream, patientNumbersPriceTypes);
        }

        public async Task CreateCsvAsync(Stream stream, Order order)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var orderItems = order.FlattenOrderItems().Select(o => new OdooOrderItem(o));

            await csvWriter.WriteRecordsAsync(stream, orderItems);
        }
    }
}
