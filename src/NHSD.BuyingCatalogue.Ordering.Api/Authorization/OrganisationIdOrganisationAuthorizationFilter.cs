using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.BuyingCatalogue.Ordering.Api.Authorization
{
    internal sealed class OrganisationIdOrganisationAuthorizationFilter : OrganisationAuthorizationFilter
    {
        internal const string DefaultParameterName = "organisationId";

        protected override string RouteParameterName => DefaultParameterName;

        protected override IEnumerable<string> ActionMethodParameterNames => new[] { DefaultParameterName };

        protected override Task<(string Id, IActionResult Result)> GetOrganisationId(string routeValue)
        {
            return Task.FromResult<(string, IActionResult)>((routeValue, null));
        }
    }
}
