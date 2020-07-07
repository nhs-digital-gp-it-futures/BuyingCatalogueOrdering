using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    public static class OrderExtensions
    {
        public static bool IsOrderingPartySectionComplete(this Order order)
        {
            return order?.OrganisationContact != null;
        }

        public static bool IsSupplierSectionComplete(this Order order)
        {
            return order?.SupplierContact != null;
        }

        public static bool IsCommencementDateSectionComplete(this Order order)
        {
            return order?.CommencementDate != null;
        }

        public static bool IsServiceRecipientsSectionComplete(this Order order)
        {
            if (order == null)
            {
                return false;
            }
            return order.ServiceRecipientsViewed;
        }

        public static bool IsCatalogueSolutionsSectionComplete(this Order order)
        {
            if (order == null)
            {
                return false;
            }
            return order.CatalogueSolutionsViewed;
        }

        public static bool IsAdditionalServicesSectionComplete(this Order order)
        {
            if (order == null)
            {
                return false;
            }
            return order.AdditionalServicesViewed;
        }
    }
}
