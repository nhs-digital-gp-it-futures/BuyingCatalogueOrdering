using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public sealed class CreateOrderItemRequest
    {
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
            TimeUnit? priceTimeUnit,
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
            ProvisioningType = OrderingEnums.Parse<ProvisioningType>(provisioningTypeName);
            CataloguePriceType = OrderingEnums.Parse<CataloguePriceType>(cataloguePriceTypeName);
            CataloguePriceUnitTierName = cataloguePriceUnitTierName;
            CataloguePriceUnitDescription = cataloguePriceUnitDescription;
            PriceTimeUnit = priceTimeUnit;
            CurrencyCode = currencyCode;
            Quantity = quantity;
            EstimationPeriod = OrderingEnums.ParseTimeUnit(estimationPeriodName);
            DeliveryDate = deliveryDate;
            Price = price;
        }

        public Order Order { get; }

        public string OdsCode { get; }

        public string CatalogueItemId { get; }

        public CatalogueItemType CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public string CatalogueSolutionId { get; }

        public ProvisioningType? ProvisioningType { get; }

        public CataloguePriceType? CataloguePriceType { get; }

        public string CataloguePriceUnitTierName { get; }

        public string CataloguePriceUnitDescription { get; }

        public TimeUnit? PriceTimeUnit { get; }

        public string CurrencyCode { get; }

        public int Quantity { get; }

        public TimeUnit? EstimationPeriod { get; }

        public DateTime? DeliveryDate { get; }

        public decimal? Price { get; }
    }
}
