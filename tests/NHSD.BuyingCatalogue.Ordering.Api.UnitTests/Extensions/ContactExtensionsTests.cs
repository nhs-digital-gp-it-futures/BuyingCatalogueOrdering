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
    public sealed class ContactExtensionsTests
    {
        [Test]
        public void ToModel_NullContact_ReturnsNull()
        {
            ContactExtensions.ToModel(null).Should().BeNull();
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

        [Test]
        public void FromModel_NullPrimaryContactModelModel_ThrowsException()
        {
            Contact contact = ContactBuilder
                .Create()
                .Build();

            Assert.Throws<ArgumentNullException>(() =>
            {
                contact.FromModel(null);
            });
        }

        [Test]
        public void FromModel_PrimaryContactModel_UpdatesContact()
        {
            Contact contact = ContactBuilder
                .Create()
                .Build();

            PrimaryContactModel inputPrimaryContactModel = new PrimaryContactModel
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                EmailAddress = Guid.NewGuid().ToString(),
                TelephoneNumber = Guid.NewGuid().ToString()
            };

            var actualContact = contact.FromModel(inputPrimaryContactModel);

            Contact expectedContact = ContactBuilder
                .Create()
                .WithFirstName(inputPrimaryContactModel.FirstName)
                .WithLastName(inputPrimaryContactModel.LastName)
                .WithEmail(inputPrimaryContactModel.EmailAddress)
                .WithPhone(inputPrimaryContactModel.TelephoneNumber)
                .Build();

            actualContact.Should().BeEquivalentTo(expectedContact);
        }

        [Test]
        public void FromModel_NullContact_ReturnsNewContact()
        {
            var primaryContactModel = new PrimaryContactModel();

            var actualContact = ContactExtensions.FromModel(null, primaryContactModel);

            actualContact.Should().NotBeNull();
        }
    }
}
