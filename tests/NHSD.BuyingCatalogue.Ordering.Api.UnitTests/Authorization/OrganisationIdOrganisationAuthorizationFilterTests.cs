using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Authorization
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrganisationIdOrganisationAuthorizationFilterTests
    {
        [Test]
        [OrderingAutoData]
        public static async Task OnAuthorizationAsync_UserHasDifferentPrimaryOrganisationId_ReturnsExpectedValue(
            string organisationId,
            OrganisationIdOrganisationAuthorizationFilter filter)
        {
            const string parameterName = OrganisationIdOrganisationAuthorizationFilter.DefaultParameterName;

            var user = ClaimsPrincipalBuilder.Create()
                .WithClaim(ApplicationClaimTypes.Ordering)
                .WithClaim(UserClaimTypes.PrimaryOrganisationId)
                .Build();

            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = parameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(parameterName, organisationId)
                .WithUser(user)
                .Build();

            await filter.OnAuthorizationAsync(context);

            context.Result.Should().NotBeNull();
            context.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        [OrderingAutoData]
        public static async Task OnAuthorizationAsync_UserHasSamePrimaryOrganisationId_ReturnsExpectedValue(
            string organisationId,
            OrganisationIdOrganisationAuthorizationFilter filter)
        {
            const string parameterName = OrganisationIdOrganisationAuthorizationFilter.DefaultParameterName;

            var user = ClaimsPrincipalBuilder.Create()
                .WithClaim(ApplicationClaimTypes.Ordering)
                .WithClaim(UserClaimTypes.PrimaryOrganisationId, organisationId)
                .Build();

            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = parameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(parameterName, organisationId)
                .WithUser(user)
                .Build();

            await filter.OnAuthorizationAsync(context);

            context.Result.Should().BeNull();
        }
    }
}
