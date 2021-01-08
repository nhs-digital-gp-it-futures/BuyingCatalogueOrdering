using System;
using EnumsNET;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public static class CatalogueItemTypeExtensions
    {
        public static string DisplayName(this CatalogueItemType itemType) => itemType.AsString(EnumFormat.DisplayName);

        public static TimeUnit? InferEstimationPeriod(this CatalogueItemType itemType, ProvisioningType provisioningType, TimeUnit? estimationPeriod)
        {
            return itemType switch
            {
                CatalogueItemType.AssociatedService => provisioningType == ProvisioningType.OnDemand
                    ? provisioningType.InferEstimationPeriod(estimationPeriod)
                    : null,
                _ => provisioningType.InferEstimationPeriod(estimationPeriod),
            };
        }

        public static void MarkOrderSectionAsViewed(this CatalogueItemType itemType, Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            switch (itemType)
            {
                case CatalogueItemType.Solution:
                    order.CatalogueSolutionsViewed = true;
                    break;

                case CatalogueItemType.AdditionalService:
                    order.AdditionalServicesViewed = true;
                    break;

                case CatalogueItemType.AssociatedService:
                    order.AssociatedServicesViewed = true;
                    break;

                default:
                    throw new ArgumentException($"{nameof(itemType)} must be a valid {nameof(CatalogueItemType)}", nameof(itemType));
            }
        }
    }
}
