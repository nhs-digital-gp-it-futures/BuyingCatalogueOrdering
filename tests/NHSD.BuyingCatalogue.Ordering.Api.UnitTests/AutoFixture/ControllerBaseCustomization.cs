using System;
using System.Security.Claims;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class ControllerBaseCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new Postprocessor(
                        new MethodInvoker(new ModestConstructorQuery()),
                        new ControllerBaseSpecimenCommand()),
                    new ControllerBaseRequestSpecification()));
        }

        private class ControllerBaseSpecimenCommand : ISpecimenCommand
        {
            public void Execute(object specimen, ISpecimenContext context)
            {
                if (specimen == null)
                    throw new ArgumentNullException(nameof(specimen));

                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (specimen is ControllerBase controller)
                {
                    var httpContextMock = context.Create<Mock<HttpContext>>();
                    httpContextMock
                        .Setup(c => c.User)
                        .Returns(CreateClaimsPrincipal(GetOrganisationId(context)));

                    controller.ControllerContext = new ControllerContext
                    {
                        HttpContext = httpContextMock.Object,
                    };
                }
                else
                {
                    throw new ArgumentException("The specimen must be an instance of ControllerBase", nameof(specimen));
                }
            }

            private static ClaimsPrincipal CreateClaimsPrincipal(Guid organisationId)
            {
                var claims = new[]
                {
                    new Claim("primaryOrganisationId", organisationId.ToString()),
                };

                return new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            }

            private static Guid GetOrganisationId(ISpecimenContext context)
            {
                // Order must be frozen for this to work correctly
                var order = context.Create<Order>();
                return order.OrganisationId;
            }
        }

        private class ControllerBaseRequestSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request) =>
                request is Type type && typeof(ControllerBase).IsAssignableFrom(type);
        }
    }
}
