using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Authorization
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderLookupOrganisationAuthorizationFilterTests
    {
        [Test]
        [OrderingAutoData]
        public static async Task OnAuthorizationAsync_OrderNotFound_ReturnsExpectedValue(
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
        [OrderingAutoData]
        public static async Task OnAuthorizationAsync_LooksUpExpectedOrderId(
            string orderId,
            [Frozen] Mock<IOrderRepository> orderRepositoryMock,
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
                .WithRouteValue(parameterName, orderId)
                .WithUser(user)
                .Build();

            await filter.OnAuthorizationAsync(context);

            orderRepositoryMock.Verify(r => r.GetOrderByIdAsync(
                It.Is<string>(s => s == orderId),
                It.IsNotNull<Action<IOrderQuery>>()));
        }

        [Test]
        [OrderingAutoData]
        public static async Task OnAuthorizationAsync_UsesExpectedOrderQuery(
            Mock<IOrderQuery> orderQueryMock,
            [Frozen] Mock<IOrderRepository> orderRepositoryMock,
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
                .WithRouteValue(parameterName, string.Empty)
                .WithUser(user)
                .Build();

            Action<IOrderQuery> queryConfigured = null;
            orderRepositoryMock
                .Setup(r => r.GetOrderByIdAsync(It.IsAny<string>(), It.IsNotNull<Action<IOrderQuery>>()))
                .Callback<string, Action<IOrderQuery>>((_, configureQuery) => queryConfigured = configureQuery);

            await filter.OnAuthorizationAsync(context);
            queryConfigured(orderQueryMock.Object);

            orderQueryMock.Verify(q => q.WithoutTracking());
            orderQueryMock.VerifyNoOtherCalls();
        }

        [Test]
        [OrderingAutoData]
        public static async Task OnAuthorizationAsync_UserHasSamePrimaryOrganisationId_ReturnsExpectedValue(
            Order order,
            [Frozen] Mock<IOrderRepository> orderRepositoryMock,
            OrderLookupOrganisationAuthorizationFilter filter)
        {
            const string parameterName = OrderLookupOrganisationAuthorizationFilter.DefaultParameterName;
            var orderId = order.OrderId;

            var user = ClaimsPrincipalBuilder.Create()
                .WithClaim(ApplicationClaimTypes.Ordering)
                .WithClaim(UserClaimTypes.PrimaryOrganisationId, order.OrganisationId.ToString())
                .Build();

            var actionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new object[] { new AuthorizeOrganisationAttribute() },
                Parameters = new[] { new ParameterDescriptor { Name = parameterName } },
            };

            var context = AuthorizationFilterContextBuilder.Create()
                .WithActionDescription(actionDescriptor)
                .WithRouteValue(parameterName, orderId)
                .WithUser(user)
                .Build();

            orderRepositoryMock
                .Setup(r => r.GetOrderByIdAsync(It.Is<string>(s => s == orderId), It.IsNotNull<Action<IOrderQuery>>()))
                .ReturnsAsync(order);

            await filter.OnAuthorizationAsync(context);

            context.Result.Should().BeNull();
        }
    }
}
