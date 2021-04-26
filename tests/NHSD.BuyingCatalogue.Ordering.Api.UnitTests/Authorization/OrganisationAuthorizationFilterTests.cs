using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Authorization
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrganisationAuthorizationFilterTests
    {
        private const string DefaultRouteParameterName = "aParameter";
        private const string DefaultActionMethodParameterName = "anotherParameter";

        [Test]
        public static async Task OnAuthorizationAsync_NoAttribute_ReturnsExpectedValue()
        {
            var context = AuthorizationFilterContextBuilder.Create().Build();

            var result = new OkResult();
            context.Result = result;

            var filter = new TestFilter();
            await filter.OnAuthorizationAsync(context);

            context.Result.Should().Be(result);
        }

        [Test]
        public static async Task OnAuthorizationAsync_NoParameter_ReturnsExpectedValue()
        {
            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .Build();

            var filter = new TestFilter();
            await filter.OnAuthorizationAsync(context);

            context.Result.Should().BeNull();
        }

        [Test]
        public static async Task OnAuthorizationAsync_NoUser_ReturnsExpectedValue()
        {
            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = DefaultActionMethodParameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .Build();

            var result = new OkResult();
            context.Result = result;

            var filter = new TestFilter();
            await filter.OnAuthorizationAsync(context);

            context.Result.Should().Be(result);
        }

        [Test]
        public static async Task OnAuthorizationAsync_UserHasNoOrderingClaim_ReturnsExpectedValue()
        {
            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = DefaultActionMethodParameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithUser(new ClaimsPrincipal())
                .Build();

            var filter = new TestFilter();
            await filter.OnAuthorizationAsync(context);

            context.Result.Should().BeNull();
        }

        [Test]
        [AutoData]
        public static async Task OnAuthorizationAsync_RouteValue_IsExpectedValue(string routeValue)
        {
            var user = ClaimsPrincipalBuilder.Create().WithClaim(ApplicationClaimTypes.Ordering).Build();
            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = DefaultActionMethodParameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(DefaultRouteParameterName, routeValue)
                .WithUser(user)
                .Build();

            var filter = new TestFilter();
            await filter.OnAuthorizationAsync(context);

            filter.RouteValue.Should().Be(routeValue);
        }

        [Test]
        public static async Task OnAuthorizationAsync_ImplementationReturnsActionResult_ReturnsExpectedValue()
        {
            var user = ClaimsPrincipalBuilder.Create().WithClaim(ApplicationClaimTypes.Ordering).Build();
            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = DefaultActionMethodParameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(DefaultRouteParameterName, null)
                .WithUser(user)
                .Build();

            var result = new OkResult();
            context.Result = result;
            var expectedResult = new NoContentResult();

            var filter = new TestFilter { Result = expectedResult };
            await filter.OnAuthorizationAsync(context);

            context.Result.Should().Be(expectedResult);
        }

        [Test]
        [AutoData]
        public static async Task OnAuthorizationAsync_UserHasNoPrimaryOrganisationIdClaim_ReturnsExpectedValue(
            string organisationId)
        {
            var user = ClaimsPrincipalBuilder.Create().WithClaim(ApplicationClaimTypes.Ordering).Build();
            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = DefaultActionMethodParameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(DefaultRouteParameterName, null)
                .WithUser(user)
                .Build();

            var filter = new TestFilter { Id = organisationId };
            await filter.OnAuthorizationAsync(context);

            context.Result.Should().NotBeNull();
            context.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        [AutoData]
        public static async Task OnAuthorizationAsync_UserHasDifferentPrimaryOrganisationId_ReturnsExpectedValue(
            string organisationId1,
            string organisationId2)
        {
            var user = ClaimsPrincipalBuilder.Create()
                .WithClaim(ApplicationClaimTypes.Ordering)
                .WithClaim(UserClaimTypes.PrimaryOrganisationId, organisationId1)
                .Build();

            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = DefaultActionMethodParameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(DefaultRouteParameterName, null)
                .WithUser(user)
                .Build();

            var filter = new TestFilter { Id = organisationId2 };
            await filter.OnAuthorizationAsync(context);

            context.Result.Should().NotBeNull();
            context.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        [AutoData]
        public static async Task OnAuthorizationAsync_UserHasSamePrimaryOrganisationId_ReturnsExpectedValue(
            string organisationId)
        {
            var user = ClaimsPrincipalBuilder.Create()
                .WithClaim(ApplicationClaimTypes.Ordering)
                .WithClaim(UserClaimTypes.PrimaryOrganisationId, organisationId)
                .Build();

            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = DefaultActionMethodParameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(DefaultRouteParameterName, null)
                .WithUser(user)
                .Build();

            var filter = new TestFilter { Id = organisationId };
            await filter.OnAuthorizationAsync(context);

            context.Result.Should().BeNull();
        }

        private sealed class TestFilter : OrganisationAuthorizationFilter
        {
            internal string Id { get; init; }

            internal IActionResult Result { get; init; }

            internal string RouteValue { get; private set; }

            protected override string RouteParameterName => DefaultRouteParameterName;

            protected override IEnumerable<string> ActionMethodParameterNames => new[] { DefaultActionMethodParameterName };

            protected override Task<(string Id, IActionResult Result)> GetOrganisationId(string routeValue)
            {
                RouteValue = routeValue;
                return Task.FromResult((Id, Result));
            }
        }
    }
}
