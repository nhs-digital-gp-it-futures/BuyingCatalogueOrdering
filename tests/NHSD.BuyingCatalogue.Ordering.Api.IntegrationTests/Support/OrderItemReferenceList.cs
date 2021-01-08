using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class OrderItemReferenceList
    {
        private readonly Dictionary<string, OrderItemEntity> _cache = new();

        public IEnumerable<OrderItemEntity> GetAll() =>
            _cache.Values;

        public OrderItemEntity GetByOrderCatalogueItemName(string catalogueItemName) =>
            _cache[catalogueItemName]
                .Should()
                .NotBeNull()
                .And
                .Subject
                .Should()
                .BeOfType<OrderItemEntity>()
                .Which;

        public void Add(string catalogueItemName, OrderItemEntity entity)
        {
            _cache.ContainsKey(catalogueItemName).Should().BeFalse();
            _cache.Add(catalogueItemName, entity);
        }

        public IEnumerable<OrderItemEntity> FindByOrderId(string orderId) => _cache.Values.Where(x =>
            string.Equals(x.OrderId, orderId, StringComparison.OrdinalIgnoreCase));

        public OrderItemEntity FindByOrderItemId(int? orderItemId) =>
            _cache.Values.SingleOrDefault(x => x.OrderItemId == orderItemId.GetValueOrDefault());
    }
}
