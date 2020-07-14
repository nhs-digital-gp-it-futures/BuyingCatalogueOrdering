using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class GetCatalogueSolutionOrderItemModel
    {
        public ServiceRecipientModel ServiceRecipient { get; set; }

        public string CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int Quantity { get; set; }

        public string EstimationPeriod { get; set; }

        public string ProvisioningType { get; set; }

        public string Type { get; set; }

        public string CurrencyCode { get; set; }

        public ItemUnitModel ItemUnit { get; set; }

        public decimal? Price { get; set; }

        public TimeUnitModel TimeUnit { get; set; }
    }
}
