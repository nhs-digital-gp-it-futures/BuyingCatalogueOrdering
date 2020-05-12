using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrdersBuilder
    {
        private readonly Order _order;

        private OrdersBuilder()
        {
            _order = new Order()
            {
                OrderId = "C000014-01",
                Description = "Some Description",
                OrganisationId = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                LastUpdatedBy = Guid.NewGuid(),
                OrderStatus = new OrderStatus() {OrderStatusId = 1, Name = "Submitted"}
            };
        }

        internal static OrdersBuilder Create() => new OrdersBuilder();

        internal OrdersBuilder WithOrderId(string orderId)
        {
            _order.OrderId = orderId;
            return this;
        }

        internal OrdersBuilder WithDescription(string description)
        {
            _order.Description = description;
            return this;
        }

        internal OrdersBuilder WithOrganisationId(Guid organisationId)
        {
            _order.OrganisationId = organisationId;
            return this;
        }

        internal OrdersBuilder WithCreated(DateTime created)
        {
            _order.Created = created;
            return this;
        }

        internal OrdersBuilder WithLastUpdated(DateTime lastUpdated)
        {
            _order.LastUpdated = lastUpdated;
            return this;
        }

        internal OrdersBuilder WithLastUpdatedBy(Guid lastUpdatedBy)
        {
            _order.LastUpdatedBy = lastUpdatedBy;
            return this;
        }

        internal OrdersBuilder WithOrderStatus(OrderStatus orderStatus)
        {
            _order.OrderStatus = orderStatus;
            return this;
        }

        internal Order Build() => _order;
    }
}
