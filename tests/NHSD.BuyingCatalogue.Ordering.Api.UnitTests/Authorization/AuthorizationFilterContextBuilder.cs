using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Authorization
{
    internal sealed class AuthorizationFilterContextBuilder
    {
        private readonly HttpContext httpContext = new DefaultHttpContext();
        private readonly RouteValueDictionary routeValues = new();

        private ActionDescriptor actionDescriptor = new();

        private AuthorizationFilterContextBuilder()
        {
        }

        internal static AuthorizationFilterContextBuilder Create() => new();

        internal AuthorizationFilterContextBuilder WithActionDescription(ActionDescriptor descriptor)
        {
            actionDescriptor = descriptor;
            return this;
        }

        internal AuthorizationFilterContextBuilder WithRouteValue(string key, string value)
        {
            routeValues.Add(key, value);
            return this;
        }

        internal AuthorizationFilterContextBuilder WithUser(ClaimsPrincipal user)
        {
            httpContext.User = user;
            return this;
        }

        internal AuthorizationFilterContext Build()
        {
            return new(
                new ActionContext(httpContext, new RouteData(routeValues), actionDescriptor, new ModelStateDictionary()),
                new List<IFilterMetadata>());
        }
    }
}
