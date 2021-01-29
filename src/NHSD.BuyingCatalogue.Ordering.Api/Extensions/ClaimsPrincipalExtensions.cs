using System;
using System.Security.Claims;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        private const string PrimaryOrganisationIdType = "primaryOrganisationId";

        public static Guid GetPrimaryOrganisationId(this ClaimsPrincipal user)
        {
            Claim primaryOrganisation = user.FindFirst(PrimaryOrganisationIdType);
            if (primaryOrganisation is null)
                throw new InvalidOperationException($"User does not have the {PrimaryOrganisationIdType} claim.");

            return new Guid(primaryOrganisation.Value);
        }

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return new(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            if (user.Identity is null)
                throw new InvalidOperationException($"{nameof(ClaimsPrincipal.Identity)} is null.");

            return user.Identity.Name;
        }
    }
}
