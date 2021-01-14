using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class ServiceRecipientReferenceList
    {
        private readonly Dictionary<string, ServiceRecipientEntity> cache = new();

        public void Add(ServiceRecipientEntity entity)
        {
            string key = GenerateKey(entity.OrderId, entity.OdsCode);
            cache.ContainsKey(key).Should().BeFalse();
            cache.Add(key, entity);
        }

        public IEnumerable<ServiceRecipientEntity> FindByOrderId(string orderId) =>
            cache.Values.Where(x => string.Equals(x.OrderId, orderId, StringComparison.OrdinalIgnoreCase));

        public ServiceRecipientEntity Get(string orderId, string odsCode)
        {
            string key = GenerateKey(orderId, odsCode);
            return cache[key];
        }

        private static string GenerateKey(string orderId, string odsCode)
        {
            return $"{orderId?.ToUpperInvariant()}_{odsCode?.ToUpperInvariant()}";
        }
    }
}
