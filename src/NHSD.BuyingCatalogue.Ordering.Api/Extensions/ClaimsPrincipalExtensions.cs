using System.Security.Claims;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        private const string PrimaryOrganisationIdType = "primaryOrganisationId";

        public static string GetPrimaryOrganisationId(this ClaimsPrincipal user)
        {
            return user.FindFirst(PrimaryOrganisationIdType).Value;
        }
    }
}
