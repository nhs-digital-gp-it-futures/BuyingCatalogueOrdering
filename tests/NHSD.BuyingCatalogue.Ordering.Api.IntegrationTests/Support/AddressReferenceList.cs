using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class AddressReferenceList
    {
        private readonly Dictionary<int, AddressEntity> _cache = new Dictionary<int, AddressEntity>();

        public IEnumerable<AddressEntity> GetAll() =>
            _cache.Values;

        public AddressEntity GetByAddressId(int? addressId)
        {
            if (addressId == null)
                return null;

            return _cache[addressId.Value];
        }

        public AddressEntity GetByPostcode(string postcode) =>
            GetAll()
                .FirstOrDefault(addressEntity => 
                    string.Equals(postcode, addressEntity.Postcode, StringComparison.OrdinalIgnoreCase));

        public void Add(AddressEntity addressEntity)
        {
            int addressId = addressEntity.AddressId;

            _cache.ContainsKey(addressId).Should().BeFalse();
            _cache.Add(addressId, addressEntity);
        }
    }
}
