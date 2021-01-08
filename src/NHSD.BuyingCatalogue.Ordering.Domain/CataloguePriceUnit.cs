using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class CataloguePriceUnit : ValueObject
    {
        public string Name { get; }

        public string Description { get; }

        private CataloguePriceUnit()
        {
        }

        private CataloguePriceUnit(
            string name,
            string description)
            : this()
        {
            Name = name;
            Description = description;
        }

        public static CataloguePriceUnit Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));
            }

            return new CataloguePriceUnit(name, description);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Description;
        }
    }
}
