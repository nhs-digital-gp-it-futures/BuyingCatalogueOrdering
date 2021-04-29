using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class ClaimsPrincipalExtensionsTests
    {
        [Test]
        [AutoData]
        public static void GetPrimaryOrganisationId_NoPrimaryOrganisationIdClaims_ThrowsException(
            ClaimsPrincipal user)
        {
            Assert.Throws<InvalidOperationException>(() => _ = user.GetPrimaryOrganisationId());
        }

        [Test]
        [AutoData]
        public static void GetPrimaryOrganisationId_WithPrimaryOrganisationIdClaims_ReturnsExpectedValue(
            Guid organisationId)
        {
            var claims = new[]
            {
                new Claim("primaryOrganisationId", organisationId.ToString()),
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            user.GetPrimaryOrganisationId().Should().Be(organisationId);
        }

        [Test]
        [AutoData]
        public static void GetUserId_ReturnsExpectedValue(
            Guid id)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            user.GetUserId().Should().Be(id);
        }

        [Test]
        [AutoData]
        public static void GetUserName_NullIdentity_ThrowsException(
            ClaimsPrincipal user)
        {
            Assert.Throws<InvalidOperationException>(() => _ = user.GetUserName());
        }

        [Test]
        [AutoData]
        public static void GetUserName_ReturnsExpectedValue(
            string name)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            user.GetUserName().Should().Be(name);
        }

        [Test]
        [AutoData]
        public static void IsAuthorisedForOrganisation_OrganisationIdIsPrimary_ReturnsTrue(            string name)
        {
            var claims = new[]
            {
                new Claim(UserClaimTypes.PrimaryOrganisationId, name),
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            user.IsAuthorisedForOrganisation(name).Should().BeTrue();
        }

        [Test]
        [AutoData]
        public static void IsAuthorisedForOrganisation_OrganisationIdIsRelated_ReturnsTrue(
            string name)
        {
            var claims = new[]
            {
                new Claim(UserClaimTypes.RelatedOrganisationId, name),
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            user.IsAuthorisedForOrganisation(name).Should().BeTrue();
        }

        [Test]
        [AutoData]
        public static void IsAuthorisedForOrganisation_OrganisationIdNotRelatedOrPrimary_ReturnsFalse(
            string name)
        {
            var claims = new[]
            {
                new Claim("some-claim", "some-value"),
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            user.IsAuthorisedForOrganisation(name).Should().BeFalse();
        }
    }
}
