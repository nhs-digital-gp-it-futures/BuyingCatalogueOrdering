using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class ContactExtensions
    {
        internal static PrimaryContactModel ToModel(this Contact contact)
        {
            if (contact is null)
                return null;

            return new PrimaryContactModel
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                EmailAddress = contact.Email,
                TelephoneNumber = contact.Phone
            };
        }

        internal static void FromModel(this Contact contact, PrimaryContactModel model)
        {
            if (contact is null)
                contact = new Contact();

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;
            contact.Email = model.EmailAddress;
            contact.Phone = model.TelephoneNumber;
        }
    }
}
