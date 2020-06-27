using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services.CreateOrder;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrdersControllerBuilder
    {
        private IOrderRepository _orderRepository;
        private IServiceRecipientRepository _serviceRecipientRepository;
        private ICreateOrderService _createOrderService;

        private OrdersControllerBuilder()
        {
        }

        internal static OrdersControllerBuilder Create()
        {
            return new OrdersControllerBuilder();
        }

        internal OrdersControllerBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        internal OrdersControllerBuilder WithServiceRecipientRepository(IServiceRecipientRepository serviceRecipientRepository)
        {
            _serviceRecipientRepository = serviceRecipientRepository;
            return this;
        }

        internal OrdersControllerBuilder WithCreateOrderService(ICreateOrderService createOrderService)
        {
            _createOrderService = createOrderService;
            return this;
        }

        internal OrdersController Build()
        {
            return new OrdersController(_orderRepository, _createOrderService, _serviceRecipientRepository);
        }
    }
}
