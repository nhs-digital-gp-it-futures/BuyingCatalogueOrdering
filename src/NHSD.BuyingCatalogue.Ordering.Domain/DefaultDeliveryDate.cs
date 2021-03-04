using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class DefaultDeliveryDate : IEquatable<DefaultDeliveryDate>
    {
        public int OrderId { get; init; }

        public CatalogueItemId CatalogueItemId { get; init; }

        public DateTime? DeliveryDate { get; set; }

        public bool Equals(DefaultDeliveryDate other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return OrderId == other.OrderId && CatalogueItemId == other.CatalogueItemId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DefaultDeliveryDate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderId, CatalogueItemId);
        }
    }
}
