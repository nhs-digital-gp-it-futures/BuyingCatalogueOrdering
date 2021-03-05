using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AddressExtensionsTests
    {
        [Test]
        public static void ToModel_NullAddress_ReturnsNull()
        {
            AddressExtensions.ToModel(null).Should().BeNull();
        }

        [Test]
        [AutoData]
        public static void ToModel_Address_ReturnsExpectedAddressModel(Address address)
        {
            var actual = address.ToModel();

            actual.Should().BeEquivalentTo(address, o => o.Excluding(a => a.Id));
        }

        [Test]
        public static void ToDomain_NullModel_ReturnsNull()
        {
            AddressExtensions.ToDomain(null).Should().BeNull();
        }

        [Test]
        [AutoData]
        public static void ToDomain_Address_ReturnsExpectedAddress(AddressModel model)
        {
            var actual = model.ToDomain();

            actual.Should().BeEquivalentTo(model);
        }
    }
}
