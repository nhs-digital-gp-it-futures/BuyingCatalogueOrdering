using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.BuyingCatalogue.Ordering.Api.Authorization
{
    internal sealed class OrganisationIdOrganisationAuthorizationFilter : OrganisationAuthorizationFilter
    {
        internal const string DefaultParameterName = "organisationId";

        protected override string ParameterName { get; } = DefaultParameterName;

        protected override Task<(string Id, IActionResult Result)> GetOrganisationId(string routeValue)
        {
            return Task.FromResult<(string, IActionResult)>((routeValue, null));
        }
    }
}
