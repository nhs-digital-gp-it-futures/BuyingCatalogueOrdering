using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class OrderItemReferenceList
    {
        private readonly Dictionary<string, OrderItemEntity> cache = new();

        public IEnumerable<OrderItemEntity> GetAll() =>
            cache.Values;

        public OrderItemEntity GetByOrderCatalogueItemName(string catalogueItemName) =>
            cache[catalogueItemName]
                .Should()
                .NotBeNull()
                .And
                .Subject
                .Should()
                .BeOfType<OrderItemEntity>()
                .Which;

        public void Add(string catalogueItemName, OrderItemEntity entity)
        {
            cache.ContainsKey(catalogueItemName).Should().BeFalse();
            cache.Add(catalogueItemName, entity);
        }

        public IEnumerable<OrderItemEntity> FindByOrderId(string orderId) => cache.Values.Where(i =>
            string.Equals(i.OrderId, orderId, StringComparison.OrdinalIgnoreCase));

        public OrderItemEntity FindByOrderItemId(int? orderItemId) =>
            cache.Values.SingleOrDefault(i => i.OrderItemId == orderItemId.GetValueOrDefault());
    }
}
