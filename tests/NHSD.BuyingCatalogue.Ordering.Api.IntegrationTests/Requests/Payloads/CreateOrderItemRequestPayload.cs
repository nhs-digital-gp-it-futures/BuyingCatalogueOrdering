using System;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads
{
    internal sealed class CreateOrderItemRequestPayload
    {
        public int? OrderItemId { get; set; }

        public bool HasOrderItemId => OrderItemId.HasValue;

        public bool HasServiceRecipient { get; init; }

        public bool HasItemUnit { get; init; }

        public bool HasTimeUnit { get; init; }

        public string OdsCode { get; set; }

        public string CatalogueItemId { get; init; }

        public string CatalogueItemName { get; set; }

        public string CatalogueSolutionId { get; init; }

        public DateTime? DeliveryDate { get; init; }

        public int? Quantity { get; init; }

        public TimeUnit? EstimationPeriod { get; init; }

        public CatalogueItemType? CatalogueItemType { get; init; }

        public ProvisioningType? ProvisioningType { get; init; }

        public CataloguePriceType? CataloguePriceType { get; init; }

        public string CurrencyCode { get; init; }

        public string ItemUnitName { get; init; }

        public string ItemUnitNameDescription { get; init; }

        public string TimeUnitName { get; init; }

        public string TimeUnitDescription { get; init; }

        public decimal? Price { get; init; }

        public string ServiceRecipientName { get; set; }
    }
}
