using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            var serviceRecipientDictionary = order.ServiceRecipients.ToDictionary(x => x.OdsCode, x => x.Name);
            serviceRecipientDictionary.TryAdd(order.OrganisationOdsCode, order.OrganisationName);

            var patientNumbersPriceTypes = order.OrderItems.Select(orderItem => new OdooPatientNumbersOrderItem
            {
                CallOffAgreementId = order.OrderId,
                CallOffOrderingPartyId = order.OrganisationOdsCode,
                CallOffOrderingPartyName = order.OrganisationName,
                CallOffCommencementDate = order.CommencementDate,
                ServiceRecipientId = orderItem.OdsCode,
                ServiceRecipientName = serviceRecipientDictionary[orderItem.OdsCode],
                ServiceRecipientItemId = $"{order.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                SupplierId = order.SupplierId,
                SupplierName = order.SupplierName,
                ProductId = orderItem.CatalogueItemId,
                ProductName = orderItem.CatalogueItemName,
                ProductType = orderItem.CatalogueItemType.DisplayName(),
                QuantityOrdered = orderItem.Quantity,
                UnitOfOrder = orderItem.CataloguePriceUnit.Description,
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

            var serviceRecipientDictionary = order.ServiceRecipients.ToDictionary(x => x.OdsCode, x => x.Name);
            serviceRecipientDictionary.TryAdd(order.OrganisationOdsCode, order.OrganisationName);

            var orderItems = order.OrderItems
                .Select(o => new OdooOrderItem(order, o, serviceRecipientDictionary[o.OdsCode]));

            await csvWriter.WriteRecordsAsync(stream, orderItems);
        }
    }
}
