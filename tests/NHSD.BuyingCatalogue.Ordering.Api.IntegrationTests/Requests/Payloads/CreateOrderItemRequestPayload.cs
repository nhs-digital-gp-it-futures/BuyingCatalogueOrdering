using System;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads
{
    internal sealed class CreateOrderItemRequestPayload
    {
        public bool HasServiceRecipient { get; set; }

        public bool HasItemUnit { get; set; }

        public bool HasTimeUnit { get; set; }

        public string OdsCode { get; set; }

        public string CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public string CatalogueSolutionId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int? Quantity { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public CatalogueItemType? CatalogueItemType { get; set; }

        public ProvisioningType? ProvisioningType { get; set; }

        public CataloguePriceType? CataloguePriceType { get; set; }

        public string CurrencyCode { get; set; }

        public string ItemUnitName { get; set; }

        public string ItemUnitNameDescription { get; set; }

        public string TimeUnitName { get; set; }

        public string TimeUnitDescription { get; set; }

        public decimal? Price { get; set; }

        public string ServiceRecipientName { get; set; }
    }
}
