using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class AddressExtensionsTests
    {
        [Test]
        public void ToModel_NullAddress_ReturnsNull()
        {
            AddressExtensions.ToModel(null).Should().BeNull();
        }

        [Test]
        public void ToModel_Address_ReturnsAddressModel()
        {
            Address address = AddressBuilder
                .Create()
                .WithLine1(Guid.NewGuid().ToString())
                .WithLine2(Guid.NewGuid().ToString())
                .WithLine3(Guid.NewGuid().ToString())
                .WithLine4(Guid.NewGuid().ToString())
                .WithLine5(Guid.NewGuid().ToString())
                .WithTown(Guid.NewGuid().ToString())
                .WithCounty(Guid.NewGuid().ToString())
                .WithPostcode(Guid.NewGuid().ToString())
                .WithCountry(Guid.NewGuid().ToString())
                .Build();

            var actual = address.ToModel();

            AddressModel expected = new AddressModel
            {
                Line1 = address.Line1,
                Line2 = address.Line2,
                Line3 = address.Line3,
                Line4 = address.Line4,
                Line5 = address.Line5,
                Town = address.Town,
                County = address.County,
                Postcode = address.Postcode,
                Country = address.Country,
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void FromModel_NullAddressModel_ThrowsException()
        {
            Address address = AddressBuilder
                .Create()
                .Build();

            Assert.Throws<ArgumentNullException>(() =>
            {
                address.FromModel(null);
            });
        }

        [Test]
        public void FromModel_AddressModel_UpdatesAddress()
        {
            Address address = AddressBuilder
                .Create()
                .Build();

            AddressModel inputAddressModel = new AddressModel
            {
                Line1 = Guid.NewGuid().ToString(),
                Line2 = Guid.NewGuid().ToString(),
                Line3 = Guid.NewGuid().ToString(),
                Line4 = Guid.NewGuid().ToString(),
                Line5 = Guid.NewGuid().ToString(),
                Town = Guid.NewGuid().ToString(),
                County = Guid.NewGuid().ToString(),
                Postcode = Guid.NewGuid().ToString(),
                Country = Guid.NewGuid().ToString(),
            };

            var actualAddress = address.FromModel(inputAddressModel);

            actualAddress.Should().BeEquivalentTo(inputAddressModel);
        }

        [Test]
        public void FromModel_NullAddress_ReturnsNewAddress()
        {
            var addressModel = new AddressModel();

            var actualAddress = AddressExtensions.FromModel(null, addressModel);

            actualAddress.Should().NotBeNull();
        }
    }
}
