using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    public static class OrderExtensions
    {
        public static bool IsOrderingPartySectionComplete(this Order order)
        {
            return order?.OrganisationContact != null;
        }
    }
}
