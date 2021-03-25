using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class IdentityServiceTests
    {
        private const string UserId = "2a12304b-b721-497d-9136-12be384c1dbe";
        private const string UserName = "UserName-18e14622-187e-4d27-acca-96cd53008da0";

        [Test]
        public static void Constructor_NullHttpContextAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new IdentityService(null));
        }

        [Test]
        [OrderingAutoData(UserId, UserName)]
        public static void GetUserIdentity_ReturnsUserIdentity(IdentityService service)
        {
            var actual = service.GetUserIdentity();

            actual.Should().Be(UserId);
        }

        [Test]
        [OrderingAutoData(UserId, UserName)]
        public static void GetUserName_ReturnsUserName(IdentityService service)
        {
            var actual = service.GetUserName();

            actual.Should().Be(UserName);
        }

        [Test]
        [OrderingAutoData(UserId, UserName)]
        public static void GetUserInfo_ReturnsExpectedInfo(IdentityService service)
        {
            var actual = service.GetUserInfo();

            actual.Should().BeEquivalentTo(new { Id = Guid.Parse(UserId), Name = UserName });
        }
    }
}
