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
    public class ContactExtensionsTests
    {
        [Test]
        public void ToModel_NullContact_ReturnsNull()
        {
            Contact contact = null;
            contact.ToModel().Should().BeNull();
        }

        [Test]
        public void ToModel_Contact_ReturnsPrimaryContactModel()
        {
            Contact contact = ContactBuilder
                .Create()
                .WithFirstName(Guid.NewGuid().ToString())
                .WithLastName(Guid.NewGuid().ToString())
                .WithEmail(Guid.NewGuid().ToString())
                .WithPhone(Guid.NewGuid().ToString())
                .Build();
            
            var actual = contact.ToModel();

            PrimaryContactModel expected = new PrimaryContactModel
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                EmailAddress = contact.Email,
                TelephoneNumber = contact.Phone
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
