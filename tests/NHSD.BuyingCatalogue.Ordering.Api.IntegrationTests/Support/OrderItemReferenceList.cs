using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    [Binding]
    internal sealed class OrderItemReferenceList
    {
        private readonly Dictionary<string, OrderItemEntity> _cache = new Dictionary<string, OrderItemEntity>();

        public OrderItemEntity GetByCatalogueSolutionItemName(string catalogueSolutionItemName) => 
            _cache[catalogueSolutionItemName]
                .Should()
                .NotBeNull()
                .And
                .Subject
                .Should()
                .BeOfType<OrderItemEntity>()
                .Which;

        public void Add(string catalogueSolutionItemName, OrderItemEntity entity)
        {
            _cache.ContainsKey(catalogueSolutionItemName).Should().BeFalse();
            _cache.Add(catalogueSolutionItemName, entity);
        }
    }
}
