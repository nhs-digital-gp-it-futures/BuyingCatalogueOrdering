using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderingPartyModel
    {
        public OrderingPartyModel()
        {
        }

        internal OrderingPartyModel(OrderingParty orderingParty, Contact primaryContact)
        {
            if (orderingParty is null)
                throw new ArgumentNullException(nameof(orderingParty));

            Name = orderingParty.Name;
            OdsCode = orderingParty.OdsCode;

            var address = orderingParty.Address;
            Address = address is null
                ? null
                : new AddressModel
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

            PrimaryContact = primaryContact is null
                ? null
                : new PrimaryContactModel
                {
                    FirstName = primaryContact.FirstName,
                    LastName = primaryContact.LastName,
                    EmailAddress = primaryContact.Email,
                    TelephoneNumber = primaryContact.Phone,
                };
        }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public AddressModel Address { get; set; }

        public PrimaryContactModel PrimaryContact { get; set; }
    }
}
