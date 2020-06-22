using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class CataloguePriceUnit : ValueObject
    {
        public string TierName { get; }

        private string Description { get; }

        private CataloguePriceUnit(
            string tierName, 
            string description)
        {
            TierName = tierName;
            Description = description;
        }

        public static CataloguePriceUnit Create(string tierName, string description)
        {
            if (string.IsNullOrWhiteSpace(tierName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(tierName));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));
            }

            return new CataloguePriceUnit(tierName, description);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TierName;
            yield return Description;
        }
    }
}
