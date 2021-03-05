using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderingPartyModelTests
    {
        [Test]
        [CommonAutoData]
        public static void Constructor_NullOrderingParty_ThrowsArgumentNullException(Contact contact)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderingPartyModel(null, contact));
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesNameToExpectedValue(
            [Frozen] OrderingParty party)
        {
            var model = new OrderingPartyModel(party, null);
            model.Name.Should().Be(party.Name);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesOdsCodeToExpectedValue(
            [Frozen] OrderingParty party)
        {
            var model = new OrderingPartyModel(party, null);
            model.OdsCode.Should().Be(party.OdsCode);
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_NullAddress_InitializesAddressToExpectedValue(
            [Frozen] OrderingParty party)
        {
            party.Address = null;
            var model = new OrderingPartyModel(party, null);

            model.Address.Should().BeNull();
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesAddressToExpectedValue(
            [Frozen] OrderingParty party)
        {
            var model = new OrderingPartyModel(party, null);
            model.Address.Should().BeEquivalentTo(party.Address, o => o.Excluding(a => a.Id));
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_NullContact_InitializesContactToExpectedValue(
            OrderingParty party)
        {
            var model = new OrderingPartyModel(party, null);

            model.PrimaryContact.Should().BeNull();
        }

        [Test]
        [CommonAutoData]
        public static void Constructor_InitializesContactToExpectedValue(
            OrderingParty party,
            Contact contact)
        {
            var model = new OrderingPartyModel(party, contact);

            model.PrimaryContact.Should().BeEquivalentTo(new
            {
                contact.FirstName,
                contact.LastName,
                EmailAddress = contact.Email,
                TelephoneNumber = contact.Phone,
            });
        }
    }
}
