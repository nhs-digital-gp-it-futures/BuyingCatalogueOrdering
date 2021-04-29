using System;
using System.Linq;
using System.Security.Claims;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

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
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            return userId is null ? Guid.Empty : new Guid(userId);
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            if (user.Identity is null)
                throw new InvalidOperationException($"{nameof(ClaimsPrincipal.Identity)} is null.");

            return user.Identity.Name ?? "Chris";
        }

        public static bool IsAuthorisedForOrganisation(this ClaimsPrincipal user, string id)
        {
            var userAuthorisedOrganisations = user.FindAll(UserClaimTypes.RelatedOrganisationId)
                .Select(c => c.Value)
                .Append(user.FindFirstValue(UserClaimTypes.PrimaryOrganisationId))
                .Where(s => !string.IsNullOrEmpty(s));

            return userAuthorisedOrganisations.Any(s => s.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
