using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Authorization
{
    internal abstract class OrganisationAuthorizationFilter : IAsyncAuthorizationFilter
    {
        protected abstract string RouteParameterName { get; }

        protected abstract IEnumerable<string> ActionMethodParameterNames { get; }

        public virtual async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!ActionRequiresHandling(context.ActionDescriptor))
                return;

            var user = context.HttpContext.User;

            if (!UserHasOrderingClaim(context.HttpContext.User))
                return;

            (var isSameOrganisation, IActionResult actionResult) = await UserBelongsToRequestOrganisation(
                user,
                context.RouteData.Values);

            if (actionResult is not null)
            {
                context.Result = actionResult;
                return;
            }

            if (!isSameOrganisation)
                context.Result = new ForbidResult();
        }

        protected abstract Task<(string Id, IActionResult Result)> GetOrganisationId(string routeValue);

        private static bool UserHasOrderingClaim(ClaimsPrincipal user) => user.HasClaim(c =>
            string.Equals(c.Type, ApplicationClaimTypes.Ordering, StringComparison.OrdinalIgnoreCase));

        private bool ActionRequiresHandling(ActionDescriptor descriptor)
        {
            return descriptor.EndpointMetadata.OfType<AuthorizeOrganisationAttribute>().Any()
                && descriptor.Parameters.Any(i => ActionMethodParameterNames.Contains(i.Name));
        }

        private async Task<(bool IsSameOrganisation, IActionResult Result)> UserBelongsToRequestOrganisation(
            ClaimsPrincipal user,
            RouteValueDictionary routeValues)
        {
            (var id, IActionResult result) = await GetOrganisationId(routeValues[RouteParameterName]?.ToString() ?? string.Empty);
            if (result is not null)
                return (false, result);

            var userOrganizationId = user.FindFirstValue(UserClaimTypes.PrimaryOrganisationId);
            var isSameOrganisation = string.Equals(userOrganizationId, id, StringComparison.OrdinalIgnoreCase);

            return (isSameOrganisation, null);
        }
    }
}
