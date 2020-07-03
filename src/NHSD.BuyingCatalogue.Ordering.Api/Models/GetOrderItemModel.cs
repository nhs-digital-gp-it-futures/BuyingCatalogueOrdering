using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class GetOrderItemModel
    {
        public string ItemId { get; set; }

        public string ServiceRecipientOdsCode { get; set; }

        public string CataloguePriceType { get; set; }

        public string CatalogueItemType { get; set; }

        public string CatalogueItemName { get; set; }

        public string CatalogueItemId { get; set; }

        public string ProvisioningType { get; set; }

        public decimal Price { get; set; }

        public string ItemUnitDescription { get; set; }

        public string TimeUnitDescription { get; set; }

        public int Quantity { get; set; }

        public string QuantityPeriodDescription { get; set; }

        public DateTime DeliveryDate { get; set; }

        public decimal CostPerYear { get; set; }
    }
}
