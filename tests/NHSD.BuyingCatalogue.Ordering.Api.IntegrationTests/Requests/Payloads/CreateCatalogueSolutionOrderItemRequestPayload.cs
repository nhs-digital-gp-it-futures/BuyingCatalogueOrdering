using System;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads
{
    internal sealed class CreateCatalogueSolutionOrderItemRequestPayload
    {
        public bool HasServiceRecipient { get; set; }

        public bool HasItemUnit { get; set; }

        public string OdsCode { get; set; }

        public string CatalogueSolutionId { get; set; }

        public string CatalogueSolutionName { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int? Quantity { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public ProvisioningType? ProvisioningType { get; set; }

        public CataloguePriceType? CataloguePriceType { get; set; }

        public string CurrencyCode { get; set; }

        public string ItemUnitName { get; set; }

        public string ItemUnitNameDescription { get; set; }

        public decimal? Price { get; set; } 
    }
}
