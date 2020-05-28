using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
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
            Address address = null;
            address.ToModel().Should().BeNull();
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
                Country = address.Country
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
