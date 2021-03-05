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
    internal static class ContactExtensionsTests
    {
        [Test]
        public static void ToModel_NullContact_ReturnsNull()
        {
            ContactExtensions.ToModel(null).Should().BeNull();
        }

        [Test]
        [AutoData]
        public static void ToModel_ReturnsExpectedPrimaryContactModel(Contact contact)
        {
            var actual = contact.ToModel();

            var expected = new PrimaryContactModel
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                EmailAddress = contact.Email,
                TelephoneNumber = contact.Phone,
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static void ToDomain_NullModel_ReturnsExpectedContact()
        {
            ContactExtensions.ToDomain(null).Should().BeEquivalentTo(new Contact());
        }

        [Test]
        [AutoData]
        public static void ToDomain_ReturnsExpectedContact(PrimaryContactModel model)
        {
            var actual = model.ToDomain();

            var expected = new Contact
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.EmailAddress,
                Phone = model.TelephoneNumber,
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
