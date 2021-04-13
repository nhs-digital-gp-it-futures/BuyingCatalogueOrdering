using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    public static class OrderExtensions
    {
        public static bool IsOrderingPartySectionComplete(this Order order)
        {
            return order?.OrderingPartyContact is not null;
        }

        public static bool IsSupplierSectionComplete(this Order order)
        {
            return order?.SupplierContact is not null;
        }

        public static bool IsCommencementDateSectionComplete(this Order order)
        {
            return order?.CommencementDate is not null;
        }

        public static bool IsServiceRecipientsSectionComplete(this Order order)
        {
            return order is not null && order.Progress.ServiceRecipientsViewed;
        }

        public static bool IsCatalogueSolutionsSectionComplete(this Order order)
        {
            return order is not null && order.Progress.CatalogueSolutionsViewed;
        }

        public static bool IsAdditionalServicesSectionComplete(this Order order)
        {
            return order is not null && order.HasSolution() && order.Progress.AdditionalServicesViewed;
        }

        public static bool IsFundingSourceComplete(this Order order)
        {
            return order?.FundingSourceOnlyGms is not null && (order.HasSolution() || order.HasAdditionalService());
        }

        public static bool IsAssociatedServicesSectionComplete(this Order order)
        {
            return order is not null && order.Progress.AssociatedServicesViewed;
        }

        public static bool IsSectionStatusComplete(this Order order) =>
            order is not null && order.CanComplete();
    }
}
