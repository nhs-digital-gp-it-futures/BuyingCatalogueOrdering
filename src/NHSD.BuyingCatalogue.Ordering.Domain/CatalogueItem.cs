using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class CatalogueItem : IEquatable<CatalogueItem>
    {
        public CatalogueItemId Id { get; init; }

        public CatalogueItemType CatalogueItemType { get; init; }

        public CatalogueItemId ParentCatalogueItemId { get; set; }

        public string Name { get; set; }

        public bool Equals(CatalogueItem other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return ReferenceEquals(this, other) || Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CatalogueItem);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
