using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public sealed class PriceType
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
        public string UnitTime { get; set; }
        public string EstimationPeriod { get; set; }
        public decimal Price { get; set; }
        public int OrderType { get; set; }
        public string FundingType { get; set; } = "Central";
        public DateTime? M1Planned { get; set; }
        public DateTime? ActualM1Date { get; set; } = null;
        public DateTime? BuyerVerificationDate { get; set; } = null;
        public DateTime? CeaseDate { get; set; } = null;
    }
}
