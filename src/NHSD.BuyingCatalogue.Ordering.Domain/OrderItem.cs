using System;
using JetBrains.Annotations;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderItem : IEquatable<OrderItem>
    {
#pragma warning disable IDE0044 // Add readonly modifier

        // Cannot be read-only so that EF Core can set value
        [UsedImplicitly]
        private DateTime created;
        private DateTime lastUpdated;

        // Cannot be read-only so that EF Core can set value
        [UsedImplicitly]
        private int orderItemId;

#pragma warning restore IDE0044 // Add readonly modifier

        public OrderItem(
            string odsCode,
            string catalogueItemId,
            CatalogueItemType catalogueItemType,
            string catalogueItemName,
            string parentCatalogueItemId,
            ProvisioningType provisioningType,
            CataloguePriceType cataloguePriceType,
            CataloguePriceUnit cataloguePriceUnit,
            TimeUnit? priceTimeUnit,
            string currencyCode,
            int quantity,
            TimeUnit? estimationPeriod,
            DateTime? deliveryDate,
            decimal? price,
            int orderItemId = default)
            : this()
        {
            if (string.IsNullOrWhiteSpace(catalogueItemId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(catalogueItemId));

            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(currencyCode));

            if (string.IsNullOrWhiteSpace(catalogueItemName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(catalogueItemName));

            OdsCode = odsCode;
            CatalogueItemId = catalogueItemId;
            CatalogueItemType = catalogueItemType;
            CatalogueItemName = catalogueItemName;
            ParentCatalogueItemId = parentCatalogueItemId;
            ProvisioningType = provisioningType;
            CataloguePriceType = cataloguePriceType;
            CataloguePriceUnit = cataloguePriceUnit ?? throw new ArgumentNullException(nameof(cataloguePriceUnit));
            PriceTimeUnit = priceTimeUnit;
            CurrencyCode = currencyCode;
            Quantity = quantity;
            EstimationPeriod = estimationPeriod;
            DeliveryDate = deliveryDate;
            Price = price;
            this.orderItemId = orderItemId;

            var now = DateTime.UtcNow;
            created = now;
            lastUpdated = now;
        }

        private OrderItem()
        {
        }

        // A private field is used here as EF core will set this value when an order item is persisted to the database.
        // To mimic this functionality in the unit tests use the name of this field to set it via reflection.
        // ReSharper disable once ConvertToAutoProperty
        public int OrderItemId => orderItemId;

        public string OdsCode { get; }

        public string CatalogueItemId { get; }

        public CatalogueItemType CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public string ParentCatalogueItemId { get; }

        public ProvisioningType ProvisioningType { get; }

        public CataloguePriceType CataloguePriceType { get; }

        public CataloguePriceUnit CataloguePriceUnit { get; }

        public TimeUnit? PriceTimeUnit { get; }

        public string CurrencyCode { get; }

        public int Quantity { get; private set; }

        public TimeUnit? EstimationPeriod { get; private set; }

        public DateTime? DeliveryDate { get; private set; }

        public decimal? Price { get; private set; }

        // Backing field is set by EF Core (allowing property to be read-only)
        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public DateTime LastUpdated => lastUpdated;

        // Backing field is set by EF Core (allowing property to be read-only)
        // ReSharper disable once ConvertToAutoProperty
        public DateTime Created => created;

        public CostType CostType =>
            CatalogueItemType.Equals(CatalogueItemType.AssociatedService) &&
            ProvisioningType.Equals(ProvisioningType.Declarative)
                ? CostType.OneOff
                : CostType.Recurring;

        internal bool Updated { get; private set; }

        public decimal CalculateTotalCostPerYear()
        {
            return Price.GetValueOrDefault() * Quantity * (PriceTimeUnit?.AmountInYear() ?? EstimationPeriod?.AmountInYear() ?? 1);
        }

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

        public override int GetHashCode()
        {
            if (!IsTransient())
                return OrderItemId;

            return base.GetHashCode();
        }

        internal void UpdateFrom(OrderItem updatedItem)
        {
            ChangePrice(
                updatedItem.DeliveryDate,
                updatedItem.Quantity,
                updatedItem.EstimationPeriod,
                updatedItem.Price,
                null);
        }

        internal void ChangePrice(
            DateTime? deliveryDate,
            int quantity,
            TimeUnit? estimationPeriod,
            decimal? price,
            Action onPropertyChangedCallback)
        {
            bool changed = !Equals(DeliveryDate, deliveryDate);
            changed = changed || !Equals(Quantity, quantity);
            changed = changed || (estimationPeriod != null && !Equals(EstimationPeriod, estimationPeriod));
            changed = changed || !Equals(Price, price);

            DeliveryDate = deliveryDate;
            Quantity = quantity;

            if (estimationPeriod != null)
            {
                EstimationPeriod = estimationPeriod;
            }

            Price = price;

            if (!changed)
                return;

            onPropertyChangedCallback?.Invoke();
            lastUpdated = DateTime.UtcNow;
            Updated = true;
        }

        internal void MarkOrderSectionAsViewed(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            CatalogueItemType.MarkOrderSectionAsViewed(order);
        }

        private bool IsTransient() => OrderItemId == default;
    }
}
