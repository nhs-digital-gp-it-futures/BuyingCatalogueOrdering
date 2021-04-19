using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services
{
    internal sealed class ContactDetailsService : IContactDetailsService
    {
        public Address AddOrUpdateAddress(Address existingAddress, AddressModel newOrUpdatedAddress)
        {
            if (existingAddress is null)
                return newOrUpdatedAddress.ToDomain();

            if (newOrUpdatedAddress is null)
                return existingAddress;

            existingAddress.Line1 = newOrUpdatedAddress.Line1;
            existingAddress.Line2 = newOrUpdatedAddress.Line2;
            existingAddress.Line3 = newOrUpdatedAddress.Line3;
            existingAddress.Line4 = newOrUpdatedAddress.Line4;
            existingAddress.Line5 = newOrUpdatedAddress.Line5;
            existingAddress.Town = newOrUpdatedAddress.Town;
            existingAddress.County = newOrUpdatedAddress.County;
            existingAddress.Postcode = newOrUpdatedAddress.Postcode;
            existingAddress.Country = newOrUpdatedAddress.Country;

            return existingAddress;
        }

        public Contact AddOrUpdatePrimaryContact(Contact existingContact, PrimaryContactModel newOrUpdatedContact)
        {
            if (existingContact is null)
                return newOrUpdatedContact.ToDomain();

            if (newOrUpdatedContact is null)
                return existingContact;

            existingContact.FirstName = newOrUpdatedContact.FirstName;
            existingContact.LastName = newOrUpdatedContact.LastName;
            existingContact.Email = newOrUpdatedContact.EmailAddress;
            existingContact.Phone = newOrUpdatedContact.TelephoneNumber;

            return existingContact;
        }
    }
}
