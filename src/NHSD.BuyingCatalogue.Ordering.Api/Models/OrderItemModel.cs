using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderItemModel
    {
        internal OrderItemModel(OrderItem orderItem, IReadOnlyList<ExtendedOrderItemRecipientModel> recipients)
        {
            CataloguePriceType = orderItem.CataloguePriceType.ToString();
            CatalogueItemType = orderItem.CatalogueItem.CatalogueItemType.ToString();
            CatalogueItemName = orderItem.CatalogueItem.Name;
            ProvisioningType = orderItem.ProvisioningType.ToString();
            ItemUnitDescription = orderItem.PricingUnit.Description;
            TimeUnitDescription = orderItem.PriceTimeUnit?.Description();
            QuantityPeriodDescription = orderItem.EstimationPeriod?.Description();
            Price = orderItem.Price;
            CostPerYear = orderItem.CalculateTotalCostPerYear();
            ServiceRecipients = recipients;
        }

        public string ItemId { get; init; }

        public string CataloguePriceType { get; init; }

        public string CatalogueItemType { get; init; }

        public string CatalogueItemName { get; init; }

        public string ProvisioningType { get; init; }

        public decimal? Price { get; init; }

        public string ItemUnitDescription { get; init; }

        public string TimeUnitDescription { get; init; }

        public IReadOnlyList<ExtendedOrderItemRecipientModel> ServiceRecipients { get; init; }

        public string QuantityPeriodDescription { get; init; }

        public decimal CostPerYear { get; init; }
    }
}
