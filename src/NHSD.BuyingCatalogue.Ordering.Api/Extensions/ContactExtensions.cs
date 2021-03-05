﻿using NHSD.BuyingCatalogue.Ordering.Api.Models;
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
                TelephoneNumber = contact.Phone,
            };
        }

        internal static Contact ToDomain(this PrimaryContactModel model)
        {
            return model is null
                ? new Contact()
                : new Contact
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.EmailAddress,
                    Phone = model.TelephoneNumber,
                };
        }
    }
}
