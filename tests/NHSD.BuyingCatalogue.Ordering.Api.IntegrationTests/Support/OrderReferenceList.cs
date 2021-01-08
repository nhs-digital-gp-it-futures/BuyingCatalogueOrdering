﻿using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class OrderReferenceList
    {
        private readonly Dictionary<string, OrderEntity> _cache = new();

        public IEnumerable<OrderEntity> GetAll() =>
            _cache.Values;

        public OrderEntity GetByOrderId(string orderId) =>
            _cache[orderId]
                .Should()
                .NotBeNull()
                .And
                .Subject
                .Should()
                .BeOfType<OrderEntity>()
                .Which;

        public void Add(string orderId, OrderEntity entity)
        {
            _cache.ContainsKey(orderId).Should().BeFalse();
            _cache.Add(orderId, entity);
        }
    }
}
