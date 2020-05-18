using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrderBuilder
    {
        private readonly Order _order;

        private OrderBuilder()
        {
            _order = new Order()
            {
                OrderId = "C000014-01",
                
                OrganisationId = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                LastUpdatedBy = Guid.NewGuid(),
                OrderStatus = new OrderStatus() {OrderStatusId = 1, Name = "Submitted"}
            };

            _order.SetDescription(OrderDescription.Create("Some Description").Value);
        }

        internal static OrderBuilder Create() => new OrderBuilder();

        internal OrderBuilder WithOrderId(string orderId)
        {
            _order.OrderId = orderId;
            return this;
        }

        internal OrderBuilder WithDescription(string description)
        {
            _order.SetDescription(OrderDescription.Create(description).Value);
            return this;
        }

        internal OrderBuilder WithOrganisationId(Guid organisationId)
        {
            _order.OrganisationId = organisationId;
            return this;
        }

        internal OrderBuilder WithCreated(DateTime created)
        {
            _order.Created = created;
            return this;
        }

        internal OrderBuilder WithLastUpdated(DateTime lastUpdated)
        {
            _order.LastUpdated = lastUpdated;
            return this;
        }

        internal OrderBuilder WithLastUpdatedBy(Guid lastUpdatedBy)
        {
            _order.LastUpdatedBy = lastUpdatedBy;
            return this;
        }

        internal OrderBuilder WithOrderStatus(OrderStatus orderStatus)
        {
            _order.OrderStatus = orderStatus;
            return this;
        }

        internal Order Build() => _order;
    }
}
