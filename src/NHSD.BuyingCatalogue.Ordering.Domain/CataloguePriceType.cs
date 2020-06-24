using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class CataloguePriceType
    {
        public static readonly CataloguePriceType Flat = new CataloguePriceType(1, nameof(Flat));
        public static readonly CataloguePriceType Tiered = new CataloguePriceType(2, nameof(Tiered));

        private CataloguePriceType(
            int id, 
            string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public int Id { get; }

        public string Name { get; }

        internal static IEnumerable<CataloguePriceType> List() => 
            new[] { Flat, Tiered };

        public static CataloguePriceType FromName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return List()
                .SingleOrDefault(s => 
                    name.Equals(s.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static CataloguePriceType FromId(int id) => 
            List().SingleOrDefault(item => id == item.Id);
    }
}
