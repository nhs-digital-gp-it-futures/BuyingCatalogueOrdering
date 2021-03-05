using System;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services
{
    internal sealed class ContactDetailsService : IContactDetailsService
    {
        private readonly ApplicationDbContext context;

        public ContactDetailsService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Address AddOrUpdateAddress(Address existingAddress, AddressModel newOrUpdatedAddress)
        {
            if (existingAddress is null)
                return newOrUpdatedAddress.ToDomain();

            context.Entry(existingAddress).CurrentValues.SetValues(newOrUpdatedAddress);
            return existingAddress;
        }

        public Contact AddOrUpdatePrimaryContact(Contact existingContact, PrimaryContactModel newOrUpdatedContact)
        {
            if (existingContact is null)
                return newOrUpdatedContact.ToDomain();

            context.Entry(existingContact).CurrentValues.SetValues(newOrUpdatedContact);
            existingContact.Email = newOrUpdatedContact.EmailAddress;
            existingContact.Phone = newOrUpdatedContact.TelephoneNumber;

            return existingContact;
        }
    }
}
