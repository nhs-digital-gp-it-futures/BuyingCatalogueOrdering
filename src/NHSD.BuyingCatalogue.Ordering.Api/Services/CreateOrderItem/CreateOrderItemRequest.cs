using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public sealed class CreateOrderItemRequest
    {
        public Order Order { get; }

        public string OdsCode { get; }

        public string CatalogueItemId { get; }

        public CatalogueItemType CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public string CatalogueSolutionId { get; }

        public string ProvisioningTypeName { get; }

        public string CataloguePriceTypeName { get; }

        public string CataloguePriceUnitTierName { get; }

        public string CataloguePriceUnitDescription { get; }

        public string PriceTimeUnitName { get; }

        public string CurrencyCode { get; }

        public int Quantity { get; }

        public string EstimationPeriodName { get; }

        public DateTime? DeliveryDate { get; }

        public decimal? Price { get; }

        public CreateOrderItemRequest(
            Order order, 
            string odsCode,
            string catalogueItemId,
            CatalogueItemType catalogueItemType,
            string catalogueItemName,
            string catalogueSolutionId,
            string provisioningTypeName,
            string cataloguePriceTypeName,
            string cataloguePriceUnitTierName,
            string cataloguePriceUnitDescription,
            string priceTimeUnitName,
            string currencyCode,
            int quantity,
            string estimationPeriodName,
            DateTime? deliveryDate,
            decimal? price)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            OdsCode = odsCode;
            CatalogueItemId = catalogueItemId;
            CatalogueItemType = catalogueItemType ?? throw new ArgumentNullException(nameof(catalogueItemType));
            CatalogueItemName = catalogueItemName;
            CatalogueSolutionId = catalogueSolutionId;
            ProvisioningTypeName = provisioningTypeName;
            CataloguePriceTypeName = cataloguePriceTypeName;
            CataloguePriceUnitTierName = cataloguePriceUnitTierName;
            CataloguePriceUnitDescription = cataloguePriceUnitDescription;
            PriceTimeUnitName = priceTimeUnitName;
            CurrencyCode = currencyCode;
            Quantity = quantity;
            EstimationPeriodName = estimationPeriodName;
            DeliveryDate = deliveryDate;
            Price = price;
        }
    }
}
