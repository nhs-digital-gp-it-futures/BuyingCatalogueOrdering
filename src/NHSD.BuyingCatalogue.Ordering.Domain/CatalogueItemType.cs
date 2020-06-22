using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class CatalogueItemType
    {
        public static readonly CatalogueItemType Solution = new CatalogueItemType(1, nameof(Solution));
        public static readonly CatalogueItemType AdditionalService = new CatalogueItemType(2, nameof(AdditionalService));
        public static readonly CatalogueItemType AssociatedService = new CatalogueItemType(3, nameof(AssociatedService));

        public int Id { get; }

        public string Name { get; }

        private CatalogueItemType(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
