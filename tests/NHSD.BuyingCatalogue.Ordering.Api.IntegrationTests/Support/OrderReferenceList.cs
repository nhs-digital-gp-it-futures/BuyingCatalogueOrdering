using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class OrderReferenceList
    {
        private readonly Dictionary<string, OrderEntity> cache = new();

        public IEnumerable<OrderEntity> GetAll() =>
            cache.Values;

        public OrderEntity GetByOrderId(string orderId) =>
            cache[orderId]
                .Should()
                .NotBeNull()
                .And
                .Subject
                .Should()
                .BeOfType<OrderEntity>()
                .Which;

        public void Add(string orderId, OrderEntity entity)
        {
            cache.ContainsKey(orderId).Should().BeFalse();
            cache.Add(orderId, entity);
        }
    }
}
