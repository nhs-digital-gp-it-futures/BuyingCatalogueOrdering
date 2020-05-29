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
    public sealed class PrimaryContactModelExtensionsTests
    {
        [Test]
        public void ToObject_NullPrimaryContactModel_ReturnsNull()
        {
            PrimaryContactModel model = null;
            model.ToObject().Should().BeNull();
        }

        [Test]
        public void ToObject_PrimaryContactModel_ReturnsContact()
        {
            var model = new PrimaryContactModel
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                EmailAddress = Guid.NewGuid().ToString(),
                TelephoneNumber = Guid.NewGuid().ToString()
            };

            var actual = model.ToObject();

            var expected = new Contact
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.EmailAddress,
                Phone = model.TelephoneNumber
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
