using System;
using System.Security.Claims;

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
    }
}
