using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public sealed class CreatePurchasingDocumentService : ICreatePurchasingDocumentService
    {
        private readonly IAttachmentCsvWriter<PatientNumbersPriceType> _patientNumbersCsvWriter;

        public CreatePurchasingDocumentService(IAttachmentCsvWriter<PatientNumbersPriceType> patientNumbersCsvWriter)
        {
            _patientNumbersCsvWriter = patientNumbersCsvWriter ?? throw new ArgumentNullException(nameof(patientNumbersCsvWriter));
        }

        public async Task CreatePatientNumbersCsvAsync(Stream stream, Order order)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var serviceRecipientDictionary = order.ServiceRecipients.ToDictionary(x => x.OdsCode);

            var patientNumbersPriceTypes = order.OrderItems.Select(orderItem => new PatientNumbersPriceType
            {
                CallOffAgreementId = order.OrderId,
                CallOffOrderingPartyId = order.OrganisationOdsCode,
                CallOffOrderingPartyName = order.OrganisationName,
                CallOffCommencementDate = order.CommencementDate,
                ServiceRecipientId = orderItem.OdsCode,
                ServiceRecipientName = serviceRecipientDictionary[orderItem.OdsCode]?.Name,
                ServiceRecipientItemId = $"{orderItem.OrderItemId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                SupplierId = order.SupplierId,
                SupplierName = order.SupplierName,
                ProductId = orderItem.CatalogueItemId,
                ProductName = orderItem.CatalogueItemName,
                ProductType = orderItem.CatalogueItemType.Name,
                QuantityOrdered = orderItem.Quantity,
                UnitOfOrder = orderItem.CataloguePriceUnit.Description,
                Price = orderItem.Price.GetValueOrDefault(),
                M1Planned = orderItem.DeliveryDate,
            }).ToList();

            await _patientNumbersCsvWriter.WriteRecordsAsync(stream, patientNumbersPriceTypes);
        }
    }
}
