using System;
using System.Security.Claims;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        private const string PrimaryOrganisationIdType = "primaryOrganisationId";

        public static Guid GetPrimaryOrganisationId(this ClaimsPrincipal user)
        {
            return new Guid(user.FindFirst(PrimaryOrganisationIdType).Value);
        }

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return new Guid(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.Identity.Name;
        }
        public static void SetLastUpdated(this Order order, ClaimsPrincipal user)
        {
            order.SetLastUpdatedByName(user.GetUserName());
            order.LastUpdatedBy = user.GetUserId();
            order.LastUpdated = DateTime.UtcNow;
        }
    }
}
