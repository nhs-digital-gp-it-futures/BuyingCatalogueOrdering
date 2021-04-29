using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Authorization
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderLookupOrganisationAuthorizationFilterTests
    {
        [Test]
        [InMemoryDbAutoData]
        public static async Task OnAuthorizationAsync_InvalidCallOffId_ReturnsExpectedValue(
            OrderLookupOrganisationAuthorizationFilter filter)
        {
            const string parameterName = OrderLookupOrganisationAuthorizationFilter.DefaultParameterName;

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
                .WithRouteValue(parameterName, "InvalidOrderId")
                .WithUser(user)
                .Build();

            await filter.OnAuthorizationAsync(context);

            context.Result.Should().NotBeNull();
            context.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task OnAuthorizationAsync_OrderNotFound_ReturnsExpectedValue(
            [Frozen] CallOffId callOffId,
            OrderLookupOrganisationAuthorizationFilter filter)
        {
            const string parameterName = OrderLookupOrganisationAuthorizationFilter.DefaultParameterName;

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
                .WithRouteValue(parameterName, callOffId.ToString())
                .WithUser(user)
                .Build();

            await filter.OnAuthorizationAsync(context);

            context.Result.Should().NotBeNull();
            context.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task OnAuthorizationAsync_UserHasSamePrimaryOrganisationId_ReturnsExpectedValue(
            [Frozen] ApplicationDbContext dbContext,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderLookupOrganisationAuthorizationFilter filter)
        {
            dbContext.Order.Add(order);
            await dbContext.SaveChangesAsync();

            const string parameterName = OrderLookupOrganisationAuthorizationFilter.DefaultParameterName;

            var user = ClaimsPrincipalBuilder.Create()
                .WithClaim(ApplicationClaimTypes.Ordering)
                .WithClaim(UserClaimTypes.PrimaryOrganisationId, order.OrderingParty.Id.ToString())
                .Build();

            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = parameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(parameterName, callOffId.ToString())
                .WithUser(user)
                .Build();

            await filter.OnAuthorizationAsync(context);

            context.Result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task OnAuthorizationAsync_DifferentPrimaryOrganisationId_UserHasRelatedOrganisationClaims_ReturnsExpectedValue(
        [Frozen] ApplicationDbContext dbContext,
        [Frozen] CallOffId callOffId,
        Order order,
        OrderLookupOrganisationAuthorizationFilter filter)
        {
            dbContext.Order.Add(order);
            await dbContext.SaveChangesAsync();

            const string parameterName = OrderLookupOrganisationAuthorizationFilter.DefaultParameterName;

            var user = ClaimsPrincipalBuilder.Create()
                .WithClaim(ApplicationClaimTypes.Ordering)
                .WithClaim(UserClaimTypes.PrimaryOrganisationId, Guid.NewGuid().ToString())
                .WithClaim(UserClaimTypes.RelatedOrganisationId, order.OrderingParty.Id.ToString())
                .Build();

            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = parameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(parameterName, callOffId.ToString())
                .WithUser(user)
                .Build();

            await filter.OnAuthorizationAsync(context);

            context.Result.Should().BeNull();
        }
    }
}
