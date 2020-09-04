using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class ContactReferenceList
    {
        private readonly Dictionary<int, ContactEntity> _cache = new Dictionary<int, ContactEntity>();

        public IEnumerable<ContactEntity> GetAll() =>
            _cache.Values;

        public ContactEntity GetByContactId(int? contactId)
        {
            if (contactId == null)
                return null;

            return _cache[contactId.Value];
        }

        public ContactEntity GetByEmail(string email) =>
            GetAll()
                .FirstOrDefault(contactEntity => 
                    string.Equals(email, contactEntity.Email, StringComparison.OrdinalIgnoreCase));

        public void Add(ContactEntity contactEntity)
        {
            int contactId = contactEntity.ContactId;

            _cache.ContainsKey(contactId).Should().BeFalse();
            _cache.Add(contactId, contactEntity);
        }
    }
}
