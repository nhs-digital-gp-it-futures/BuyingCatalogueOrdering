using System;
using System.Security.Claims;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Http;
using Moq;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class HttpContextAccessorCustomization : ICustomization
    {
        private readonly Guid userId;
        private readonly string userName;

        internal HttpContextAccessorCustomization(Guid userId, string userName)
        {
            this.userId = userId;
            this.userName = userName;
        }

        public void Customize(IFixture fixture)
        {
            var id = userId == default ? fixture.Create<Guid>() : userId;
            var name = userName ?? fixture.Create<string>();

            fixture.Customize<IHttpContextAccessor>(c => new HttpContextAccessorSpecimenBuilder(id, name));
        }

        private sealed class HttpContextAccessorSpecimenBuilder : ISpecimenBuilder
        {
            private readonly Guid userId;
            private readonly string userName;

            internal HttpContextAccessorSpecimenBuilder(Guid userId, string userName)
            {
                this.userId = userId;
                this.userName = userName;
            }

            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(IHttpContextAccessor)))
                    return new NoSpecimen();

                var httpContextMock = context.Create<Mock<HttpContext>>();
                httpContextMock
                    .Setup(c => c.User)
                    .Returns(CreateClaimsPrincipal());

                var httpContextAccessorMock = context.Create<Mock<IHttpContextAccessor>>();
                httpContextAccessorMock
                    .Setup(c => c.HttpContext)
                    .Returns(httpContextMock.Object);

                return httpContextAccessorMock.Object;
            }

            private ClaimsPrincipal CreateClaimsPrincipal()
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                };

                return new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            }
        }
    }
}
