using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class ServiceRecipientReferenceList
    {
        private readonly Dictionary<string, ServiceRecipientEntity> _cache = new Dictionary<string, ServiceRecipientEntity>();

        public void Add(ServiceRecipientEntity entity)
        {
            string key = GenerateKey(entity.OrderId, entity.OdsCode);
            _cache.ContainsKey(key).Should().BeFalse();
            _cache.Add(key, entity);
        }

        public IEnumerable<ServiceRecipientEntity> FindByOrderId(string orderId) =>
            _cache.Values.Where(x => string.Equals(x.OrderId, orderId, StringComparison.OrdinalIgnoreCase));

        private string GenerateKey(string orderId, string odsCode)
        {
            return $"{orderId}_{odsCode}";
        }
    }
}
