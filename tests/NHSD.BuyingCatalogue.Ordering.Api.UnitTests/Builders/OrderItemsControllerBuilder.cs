using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrderItemsControllerBuilder
    {
        private IOrderRepository orderRepository;
        private IUpdateOrderItemService updateOrderItemRepository;
        private ICreateOrderItemService createOrderItemService;

        private OrderItemsControllerBuilder()
        {
        }

        internal static OrderItemsControllerBuilder Create() => new();

        internal OrderItemsControllerBuilder WithOrderRepository(IOrderRepository repository)
        {
            orderRepository = repository;
            return this;
        }

        internal OrderItemsControllerBuilder WithUpdateOrderItemService(IUpdateOrderItemService service)
        {
            updateOrderItemRepository = service;
            return this;
        }

        internal OrderItemsControllerBuilder WithCreateOrderItemService(ICreateOrderItemService service)
        {
            createOrderItemService = service;
            return this;
        }

        internal OrderItemsController Build() => new(
            orderRepository,
            updateOrderItemRepository,
            createOrderItemService);
    }
}
