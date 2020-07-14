using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class OrderItemReferenceList
    {
        private readonly Dictionary<string, OrderItemEntity> _cache = new Dictionary<string, OrderItemEntity>();

        public OrderItemEntity GetByOrderItemName(string ItemName) => 
            _cache[ItemName]
                .Should()
                .NotBeNull()
                .And
                .Subject
                .Should()
                .BeOfType<OrderItemEntity>()
                .Which;

        public void Add(string ItemName, OrderItemEntity entity)
        {
            _cache.ContainsKey(ItemName).Should().BeFalse();
            _cache.Add(ItemName, entity);
        }

        public IEnumerable<OrderItemEntity> FindByOrderId(string orderId) => _cache.Values.Where(x =>
            string.Equals(x.OrderId, orderId, StringComparison.OrdinalIgnoreCase));
    }
}
