using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class CatalogueItemType : IEquatable<CatalogueItemType>
    {
        public static readonly CatalogueItemType Solution = new CatalogueItemType(
            1,
            nameof(Solution),
            "Catalogue Solution",
            order => order.CatalogueSolutionsViewed = true,
            (provisioningType, estimationPeriod) => provisioningType.InferEstimationPeriod(estimationPeriod));

        public static readonly CatalogueItemType AdditionalService = new CatalogueItemType(
            2,
            nameof(AdditionalService),
            "Additional Service",
            order => order.AdditionalServicesViewed = true,
            (provisioningType, estimationPeriod) => provisioningType.InferEstimationPeriod(estimationPeriod));

        public static readonly CatalogueItemType AssociatedService = new CatalogueItemType(
            3,
            nameof(AssociatedService),
            "Associated Service",
            order => order.AssociatedServicesViewed = true,
            (provisioningType, estimationPeriod) =>
                provisioningType.Equals(ProvisioningType.OnDemand)
                    ? provisioningType.InferEstimationPeriod(estimationPeriod)
                    : null);

        private readonly Func<ProvisioningType, TimeUnit?, TimeUnit?> inferEstimationPeriodFunction;

        private CatalogueItemType(
            int id,
            string name,
            string displayName,
            Action<Order> markOrderSectionAsViewed,
            Func<ProvisioningType, TimeUnit?, TimeUnit?> inferEstimationPeriodFunction)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            MarkOrderSectionAsViewed = markOrderSectionAsViewed ?? throw new ArgumentNullException(nameof(markOrderSectionAsViewed));
            this.inferEstimationPeriodFunction = inferEstimationPeriodFunction ?? throw new ArgumentNullException(nameof(inferEstimationPeriodFunction));
        }

        public int Id { get; }

        public string Name { get; }

        public string DisplayName { get; }

        internal Action<Order> MarkOrderSectionAsViewed { get; }

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
                name.Equals(catalogueItemType.Name, StringComparison.OrdinalIgnoreCase));
        }

        public TimeUnit? InferEstimationPeriod(ProvisioningType provisioningType, TimeUnit? estimationPeriod)
        {
            return inferEstimationPeriodFunction(provisioningType, estimationPeriod);
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

        internal static IEnumerable<CatalogueItemType> List() =>
            new[] { Solution, AdditionalService, AssociatedService };
    }
}
