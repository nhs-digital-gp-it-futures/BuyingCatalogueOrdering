using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrdersControllerBuilder
    {
        private IOrderRepository _orderRepository;
        private IServiceRecipientRepository _serviceRecipientRepository;
        private ICreateOrderService _createOrderService;
        private ICompleteOrderService _completeOrderService;

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

        internal OrdersControllerBuilder WithCompleteOrderService(ICompleteOrderService completeOrderService)
        {
            _completeOrderService = completeOrderService;
            return this;
        }

        internal OrdersController Build()
        {
            return new OrdersController(_orderRepository, _createOrderService, _serviceRecipientRepository, _completeOrderService);
        }
    }
}
