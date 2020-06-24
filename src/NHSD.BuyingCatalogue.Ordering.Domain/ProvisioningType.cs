using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ProvisioningType
    {
        public static readonly ProvisioningType Patient = new ProvisioningType(1, nameof(Patient));
        public static readonly ProvisioningType Declarative = new ProvisioningType(2, nameof(Declarative));
        public static readonly ProvisioningType OnDemand = new ProvisioningType(3, nameof(OnDemand));

        private ProvisioningType(
            int id, 
            string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public int Id { get; }

        public string Name { get; }

        internal static IEnumerable<ProvisioningType> List() => 
            new[] { OnDemand, Declarative, Patient };

        public static ProvisioningType FromName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return List()
                .SingleOrDefault(s => 
                    name.Equals(s.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static ProvisioningType FromId(int id) => 
            List().SingleOrDefault(item => id == item.Id);
    }
}
