using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Application
{
    public sealed class FlattenedOrderItem
    {
        public CatalogueItem CatalogueItem { get; init; }

        public CataloguePriceType CataloguePriceType { get; init; }

        public string CurrencyCode { get; init; }

        public DateTime? DeliveryDate { get; init; }

        public TimeUnit? EstimationPeriod { get; set; }

        public int ItemId { get; init; }

        public Order Order { get; init; }

        public decimal? Price { get; set; }

        public PricingUnit PricingUnit { get; init; }

        public TimeUnit? PriceTimeUnit { get; init; }

        public ProvisioningType ProvisioningType { get; init; }

        public int Quantity { get; init; }

        public ServiceRecipient Recipient { get; init; }
    }
}
