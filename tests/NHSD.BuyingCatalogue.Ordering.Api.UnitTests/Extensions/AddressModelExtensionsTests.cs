using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class AddressModelExtensionsTests
    {
        [Test]
        public void ToObject_NullAddressModel_ReturnsNull()
        {
            AddressModel model = null;
            model.ToObject().Should().BeNull();
        }

        [Test]
        public void ToObject_AddressModel_ReturnsAddress()
        {
            AddressModel model = new AddressModel
            {
                Line1 = Guid.NewGuid().ToString(),
                Line2 = Guid.NewGuid().ToString(),
                Line3 = Guid.NewGuid().ToString(),
                Line4 = Guid.NewGuid().ToString(),
                Line5 = Guid.NewGuid().ToString(),
                Town = Guid.NewGuid().ToString(),
                County = Guid.NewGuid().ToString(),
                Postcode = Guid.NewGuid().ToString(),
                Country = "United Kingdom"
            };

            Address expected = new Address
            {
                Line1 = model.Line1,
                Line2 = model.Line2,
                Line3 = model.Line3,
                Line4 = model.Line4,
                Line5 = model.Line5,
                Town = model.Town,
                County = model.County,
                Postcode = model.Postcode,
                Country = model.Country
            };

            var actual = model.ToObject();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
