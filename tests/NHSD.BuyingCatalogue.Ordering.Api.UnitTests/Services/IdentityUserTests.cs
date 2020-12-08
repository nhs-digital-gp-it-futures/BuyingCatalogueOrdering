using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class IdentityUserTests
    {
        [Test]
        [AutoData]
        public static void Constructor_InitializesId(
            [Frozen] Guid id,
            IdentityUser user)
        {
            user.Id.Should().Be(id);
        }

        [Test]
        [AutoData]
        public static void Constructor_InitializesName(
            [Frozen] string name,
            IdentityUser user)
        {
            user.Name.Should().Be(name);
        }

        [Test]
        [AutoData]
        public static void Deconstruct_ReturnsExpectedValues(
            [Frozen] Guid expectedId,
            [Frozen] string expectedName,
            IdentityUser user)
        {
            (Guid actualId, string actualName) = user;

            actualId.Should().Be(expectedId);
            actualName.Should().Be(expectedName);
        }
    }
}
