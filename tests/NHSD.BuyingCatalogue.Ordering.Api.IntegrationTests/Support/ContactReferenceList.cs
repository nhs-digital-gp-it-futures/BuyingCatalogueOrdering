using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class ContactReferenceList
    {
        private readonly Dictionary<int, ContactEntity> cache = new();

        public IEnumerable<ContactEntity> GetAll() =>
            cache.Values;

        public ContactEntity GetByContactId(int? contactId)
        {
            return contactId is null ? null : cache[contactId.Value];
        }

        public ContactEntity GetByEmail(string email) =>
            GetAll()
                .FirstOrDefault(contactEntity =>
                    string.Equals(email, contactEntity.Email, StringComparison.OrdinalIgnoreCase));

        public void Add(ContactEntity contactEntity)
        {
            int contactId = contactEntity.ContactId;

            cache.ContainsKey(contactId).Should().BeFalse();
            cache.Add(contactId, contactEntity);
        }
    }
}
