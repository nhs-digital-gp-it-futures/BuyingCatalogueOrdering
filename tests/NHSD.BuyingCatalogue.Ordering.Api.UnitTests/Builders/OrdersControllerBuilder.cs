using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrdersControllerBuilder
    {
        private IOrderRepository _ordersRepository;
        private ICreateOrderService _createOrderService;

        private OrdersControllerBuilder()
        {
            _ordersRepository = Mock.Of<IOrderRepository>();
            _createOrderService = Mock.Of<ICreateOrderService>();

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

        internal OrdersControllerBuilder WithCreateOrderService(ICreateOrderService createOrderService)
        {
            _createOrderService = createOrderService;
            return this;
        }

        internal OrdersController Build()
        {
            return new OrdersController(_ordersRepository, _createOrderService);
        }
    }
}
