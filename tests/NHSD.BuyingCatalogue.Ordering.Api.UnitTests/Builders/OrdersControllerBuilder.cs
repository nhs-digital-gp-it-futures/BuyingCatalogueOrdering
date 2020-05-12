using System;
using System.Collections.Generic;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrdersControllerBuilder
    {
        private IOrderRepository _ordersRepository;

        private OrdersControllerBuilder()
        {
            _ordersRepository = Mock.Of<IOrderRepository>();
        }

        internal static OrdersControllerBuilder Create()
        {
            return new OrdersControllerBuilder();
        }

        internal OrdersControllerBuilder WithListOrders(IEnumerable<Order> result)
        {
            var mockOrderRepository = new Mock<IOrderRepository>();
            mockOrderRepository.Setup(x => x.ListOrdersByOrganisationIdAsync(It.IsAny<Guid>())).ReturnsAsync(result);

            _ordersRepository = mockOrderRepository.Object;
            return this;
        }

        internal OrdersController Build()
        {
            return new OrdersController(_ordersRepository);
        }
    }
}
