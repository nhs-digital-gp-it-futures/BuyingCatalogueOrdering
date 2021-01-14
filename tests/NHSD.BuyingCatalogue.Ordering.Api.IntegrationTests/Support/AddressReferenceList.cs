using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class AddressReferenceList
    {
        private readonly Dictionary<int, AddressEntity> cache = new();

        public IEnumerable<AddressEntity> GetAll() =>
            cache.Values;

        public AddressEntity GetByAddressId(int? addressId)
        {
            if (addressId == null)
                return null;

            return cache[addressId.Value];
        }

        public AddressEntity GetByPostcode(string postcode) =>
            GetAll()
                .FirstOrDefault(addressEntity =>
                    string.Equals(postcode, addressEntity.Postcode, StringComparison.OrdinalIgnoreCase));

        public void Add(AddressEntity addressEntity)
        {
            int addressId = addressEntity.AddressId;

            cache.ContainsKey(addressId).Should().BeFalse();
            cache.Add(addressId, addressEntity);
        }
    }
}
