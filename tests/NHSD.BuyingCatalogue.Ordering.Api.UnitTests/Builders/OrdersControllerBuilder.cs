using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrdersControllerBuilder
    {
        private IOrderRepository orderRepository;
        private IServiceRecipientRepository serviceRecipientRepository;
        private ICreateOrderService createOrderService;
        private ICompleteOrderService completeOrderService;

        private OrdersControllerBuilder()
        {
        }

        internal static OrdersControllerBuilder Create() => new();

        internal OrdersControllerBuilder WithOrderRepository(IOrderRepository repository)
        {
            orderRepository = repository;
            return this;
        }

        internal OrdersControllerBuilder WithServiceRecipientRepository(IServiceRecipientRepository repository)
        {
            serviceRecipientRepository = repository;
            return this;
        }

        internal OrdersControllerBuilder WithCreateOrderService(ICreateOrderService service)
        {
            createOrderService = service;
            return this;
        }

        internal OrdersControllerBuilder WithCompleteOrderService(ICompleteOrderService service)
        {
            completeOrderService = service;
            return this;
        }

        internal OrdersController Build()
        {
            return new(orderRepository, createOrderService, serviceRecipientRepository, completeOrderService);
        }
    }
}
