using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ProvisioningType : IEquatable<ProvisioningType>
    {
        public static readonly ProvisioningType Patient = new ProvisioningType(1, nameof(Patient), (_) => TimeUnit.PerMonth);
        public static readonly ProvisioningType Declarative = new ProvisioningType(2, nameof(Declarative), (_) => TimeUnit.PerYear);
        public static readonly ProvisioningType OnDemand = new ProvisioningType(3, nameof(OnDemand), (estimationPeriod) => estimationPeriod);

        private readonly Func<TimeUnit, TimeUnit> _inferEstimationPeriodFunction;

        private ProvisioningType(
            int id, 
            string name,
            Func<TimeUnit, TimeUnit> inferEstimationPeriodFunction)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));

            _inferEstimationPeriodFunction = inferEstimationPeriodFunction ?? throw new ArgumentNullException(nameof(inferEstimationPeriodFunction));
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

        public TimeUnit InferEstimationPeriod(TimeUnit estimationPeriod)
        {
            return _inferEstimationPeriodFunction.Invoke(estimationPeriod);
        }

        public bool Equals(ProvisioningType other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj) => Equals(obj as ProvisioningType);

        public override int GetHashCode() => Id;
    }
}
