using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderItem : IEquatable<OrderItem>
    {
        public int OrderItemId { get; }

        public string OdsCode { get; }

        public string CatalogueItemId { get; }

        public CatalogueItemType CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public ProvisioningType ProvisioningType { get; }

        public CataloguePriceUnit CataloguePriceUnit { get; }

        public TimeUnit PriceUnit { get; }

        public string CurrencyCode { get; }

        public int Quantity { get; }

        public TimeUnit EstimationPeriod { get; }

        public DateTime? DeliveryDate { get; }

        public decimal? Price { get; }

        public OrderItem(
            string odsCode, 
            string catalogueItemId, 
            CatalogueItemType catalogueItemType, 
            string catalogueItemName,
            ProvisioningType provisioningType, 
            CataloguePriceUnit cataloguePriceUnit,
            TimeUnit priceUnit,
            string currencyCode, 
            int quantity,
            TimeUnit estimationPeriod,
            DateTime? deliveryDate,
            decimal? price
            )
        {
            if (string.IsNullOrWhiteSpace(odsCode))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(odsCode));

            if (string.IsNullOrWhiteSpace(catalogueItemId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(catalogueItemId));

            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(currencyCode));

            if (string.IsNullOrWhiteSpace(catalogueItemName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(catalogueItemName));

            OdsCode = odsCode;
            CatalogueItemId = catalogueItemId;
            CatalogueItemType = catalogueItemType ?? throw new ArgumentNullException(nameof(catalogueItemType));
            CatalogueItemName = catalogueItemName;
            ProvisioningType = provisioningType ?? throw new ArgumentNullException(nameof(provisioningType));
            CataloguePriceUnit = cataloguePriceUnit ?? throw new ArgumentNullException(nameof(cataloguePriceUnit));
            PriceUnit = priceUnit;
            CurrencyCode = currencyCode;
            Quantity = quantity;
            EstimationPeriod = estimationPeriod;
            DeliveryDate = deliveryDate;
            Price = price;
        }

        public bool Equals(OrderItem other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return OrderItemId == other.OrderItemId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as OrderItem);
        }

        public override int GetHashCode()
        {
            return OrderItemId;
        }
    }
}
