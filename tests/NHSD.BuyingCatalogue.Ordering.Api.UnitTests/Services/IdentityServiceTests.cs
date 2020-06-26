using System;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class IdentityServiceTests
    {
        [Test]
        public void Constructor_NullHttpContextAccessor_ThrowsArgumentNullException()
        {
            static void Test()
            {
                IdentityServiceBuilder
                    .Create()
                    .WithHttpContextAccessor(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void GetUserIdentity_ReturnsUserIdentity()
        {
            var context = IdentityServiceTestContext.Setup();

            var actual = context.IdentityService.GetUserIdentity();

            actual.Should().Be(context.UserId);
        }

        [Test]
        public void GetUserName_ReturnsUserName()
        {
            var context = IdentityServiceTestContext.Setup();

            var actual = context.IdentityService.GetUserName();

            actual.Should().Be(context.UserName);
        }

        private sealed class IdentityServiceTestContext
        {
            private IdentityServiceTestContext()
            {
                UserId = Guid.NewGuid();
                UserName = "Bob";

                IdentityService = IdentityServiceBuilder
                    .Create()
                    .WithHttpContextAccessor(new HttpContextAccessor
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(
                                new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
                                    new Claim(ClaimTypes.Name, UserName)
                                }, "mock"))
                        }
                    })
                    .Build();
            }

            internal IIdentityService IdentityService { get; }

            internal string UserName { get; }

            internal Guid UserId { get; }

            internal static IdentityServiceTestContext Setup() =>
                new IdentityServiceTestContext();
        }
    }
}
