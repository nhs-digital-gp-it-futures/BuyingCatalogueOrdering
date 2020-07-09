﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class CatalogueItemType : IEquatable<CatalogueItemType>
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

        internal static IEnumerable<CatalogueItemType> List() => 
            new[] { Solution, AdditionalService, AssociatedService };

        public static CatalogueItemType FromId(int id)
        {
            return List().SingleOrDefault(
                catalogueItemType => catalogueItemType.Id.Equals(id));
        }

        public static CatalogueItemType FromName(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return List().SingleOrDefault(catalogueItemType =>
                name.Equals(catalogueItemType.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool Equals(CatalogueItemType other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj) => Equals(obj as CatalogueItemType);

        public override int GetHashCode() => Id;
    }
}
