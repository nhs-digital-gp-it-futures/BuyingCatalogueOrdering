using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderItem : IEquatable<OrderItem>
    {
        private int _orderItemId;

        /// <summary>
        /// Gets the unique ID for this instance.
        /// </summary>
        /// <remarks>
        /// A private field (<see cref="_orderItemId"/>) is used here as EF core will set this value
        /// when an <see cref="OrderItem"/> is persisted to the database. To mimic this functionality
        /// in the unit tests use the name of this field to set it via reflection.
        /// </remarks>
        public int OrderItemId
        {
            get
            {
                return _orderItemId;
            }
        }

        public string OdsCode { get; }

        public string CatalogueItemId { get; }

        public CatalogueItemType CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public ProvisioningType ProvisioningType { get; }

        public CataloguePriceType CataloguePriceType { get; }

        public CataloguePriceUnit CataloguePriceUnit { get; }

        public TimeUnit PriceTimeUnit { get; }

        public string CurrencyCode { get; }

        public int Quantity { get; }

        public TimeUnit EstimationPeriod { get; }

        public DateTime? DeliveryDate { get; }

        public decimal? Price { get; }

        private OrderItem()
        {
        }

        public OrderItem(
            string odsCode, 
            string catalogueItemId, 
            CatalogueItemType catalogueItemType, 
            string catalogueItemName,
            ProvisioningType provisioningType, 
            CataloguePriceType cataloguePriceType,
            CataloguePriceUnit cataloguePriceUnit,
            TimeUnit priceTimeUnit,
            string currencyCode, 
            int quantity,
            TimeUnit estimationPeriod,
            DateTime? deliveryDate,
            decimal? price) : this()
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
            CataloguePriceType = cataloguePriceType ?? throw new ArgumentNullException(nameof(cataloguePriceType));
            CataloguePriceUnit = cataloguePriceUnit ?? throw new ArgumentNullException(nameof(cataloguePriceUnit));
            PriceTimeUnit = priceTimeUnit;
            CurrencyCode = currencyCode;
            Quantity = quantity;
            EstimationPeriod = estimationPeriod;
            DeliveryDate = deliveryDate;
            Price = price;
        }

        private bool IsTransient() => OrderItemId == default;

        public bool Equals(OrderItem other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (IsTransient() || other.IsTransient())
                return false;

            return OrderItemId == other.OrderItemId;
        }

        public override bool Equals(object obj) => Equals(obj as OrderItem);

        public override int GetHashCode() => OrderItemId;
    }
}
