using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public sealed class OdooPatientNumbersOrderItem
    {
        public string CallOffAgreementId { get; set; }

        public string CallOffOrderingPartyId { get; set; }

        public string CallOffOrderingPartyName { get; set; }

        public DateTime? CallOffCommencementDate { get; set; }

        public string ServiceRecipientId { get; set; }

        public string ServiceRecipientName { get; set; }

        public string ServiceRecipientItemId { get; set; }

        public string SupplierId { get; set; }

        public string SupplierName { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }

        public int QuantityOrdered { get; set; }

        public string UnitOfOrder { get; set; }

        public decimal Price { get; set; }

        public string FundingType { get; set; } = "Central";

        public DateTime? M1Planned { get; set; }

        // The remaining properties are required fields in the finance CSV file but they are not populated
        // ReSharper disable once UnassignedGetOnlyAutoProperty (used by CsvHelper to generate empty column)
        public DateTime? ActualM1Date { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty (used by CsvHelper to generate empty column)
        public DateTime? BuyerVerificationDate { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty (used by CsvHelper to generate empty column)
        public DateTime? CeaseDate { get; }
    }
}
