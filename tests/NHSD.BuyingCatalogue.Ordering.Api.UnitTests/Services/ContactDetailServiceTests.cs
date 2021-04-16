using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class ContactDetailServiceTests
    {
        [Test]
        [InMemoryDbAutoData]
        public static void AddOrUpdateAddress_NullExistingAddress_NullNewAddress(
            ContactDetailsService service)
        {
            var result = service.AddOrUpdateAddress(null, null);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static void AddOrUpdateAddress_NullExistingAddress(
            AddressModel newOrUpdatedAddress,
            ContactDetailsService service)
        {
            newOrUpdatedAddress.Should().NotBeNull();

            var expected = new Address
            {
                Line1 = newOrUpdatedAddress.Line1,
                Line2 = newOrUpdatedAddress.Line2,
                Line3 = newOrUpdatedAddress.Line3,
                Line4 = newOrUpdatedAddress.Line4,
                Line5 = newOrUpdatedAddress.Line5,
                Town = newOrUpdatedAddress.Town,
                County = newOrUpdatedAddress.County,
                Postcode = newOrUpdatedAddress.Postcode,
                Country = newOrUpdatedAddress.Country,
            };

            var result = service.AddOrUpdateAddress(null, newOrUpdatedAddress);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void AddOrUpdateAddress_NullNewAddress(
            Address existingAddress,
            ContactDetailsService service)
        {
            existingAddress.Should().NotBeNull();

            var result = service.AddOrUpdateAddress(existingAddress, null);

            result.Should().BeEquivalentTo(existingAddress);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void AddOrUpdateAddress_UpdatesAddress(
            Address existingAddress,
            AddressModel newOrUpdatedAddress,
            ContactDetailsService service)
        {
            existingAddress.Should().NotBeNull();
            newOrUpdatedAddress.Should().NotBeNull();

            var expected = new Address
            {
                Id = existingAddress.Id,
                Line1 = newOrUpdatedAddress.Line1,
                Line2 = newOrUpdatedAddress.Line2,
                Line3 = newOrUpdatedAddress.Line3,
                Line4 = newOrUpdatedAddress.Line4,
                Line5 = newOrUpdatedAddress.Line5,
                Town = newOrUpdatedAddress.Town,
                County = newOrUpdatedAddress.County,
                Postcode = newOrUpdatedAddress.Postcode,
                Country = newOrUpdatedAddress.Country,
            };

            var result = service.AddOrUpdateAddress(existingAddress, newOrUpdatedAddress);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void AddOrUpdatePrimaryContact_NullExistingContact_NullNewContact(
            ContactDetailsService service)
        {
            Contact expected = new Contact();

            var result = service.AddOrUpdatePrimaryContact(null, null);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void AddOrUpdatePrimaryContact_NullExistingContact(
            PrimaryContactModel newOrUpdatedContact,
            ContactDetailsService service)
        {
            newOrUpdatedContact.Should().NotBeNull();

            var expected = new Contact
            {
                FirstName = newOrUpdatedContact.FirstName,
                LastName = newOrUpdatedContact.LastName,
                Email = newOrUpdatedContact.EmailAddress,
                Phone = newOrUpdatedContact.TelephoneNumber,
            };

            var result = service.AddOrUpdatePrimaryContact(null, newOrUpdatedContact);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void AddOrUpdatePrimaryContact_NullNewContact(
            Contact existingContact,
            ContactDetailsService service)
        {
            existingContact.Should().NotBeNull();

            var result = service.AddOrUpdatePrimaryContact(existingContact, null);

            result.Should().BeEquivalentTo(existingContact);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void AddOrUpdatePrimaryContact_UpdatesContact(
            Contact existingContact,
            PrimaryContactModel newOrUpdatedContact,
            ContactDetailsService service)
        {
            existingContact.Should().NotBeNull();
            newOrUpdatedContact.Should().NotBeNull();

            var expected = new Contact
            {
                Id = existingContact.Id,
                FirstName = newOrUpdatedContact.FirstName,
                LastName = newOrUpdatedContact.LastName,
                Email = newOrUpdatedContact.EmailAddress,
                Phone = newOrUpdatedContact.TelephoneNumber,
            };

            var result = service.AddOrUpdatePrimaryContact(existingContact, newOrUpdatedContact);

            result.Should().BeEquivalentTo(expected);
        }
    }
}
