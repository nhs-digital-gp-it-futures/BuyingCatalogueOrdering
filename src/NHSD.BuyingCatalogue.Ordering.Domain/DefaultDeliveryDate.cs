using System;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class DefaultDeliveryDate : IEquatable<DefaultDeliveryDate>
    {
        public string OrderId { get; init; }

        public string CatalogueItemId { get; init; }

        public int PriceId { get; init; }

        public DateTime DeliveryDate { get; init; }

        public bool Equals(DefaultDeliveryDate other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return OrderId.EqualsOrdinalIgnoreCase(other.OrderId)
               && CatalogueItemId.EqualsOrdinalIgnoreCase(other.CatalogueItemId)
               && PriceId == other.PriceId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DefaultDeliveryDate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderId, CatalogueItemId, PriceId);
        }
    }
}
