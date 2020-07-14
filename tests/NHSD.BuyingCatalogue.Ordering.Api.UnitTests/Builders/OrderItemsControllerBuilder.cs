using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrderItemsControllerBuilder
    {
        private IOrderRepository _orderRepository;
        private IUpdateOrderItemService _updateOrderItemRepository;

        private OrderItemsControllerBuilder()
        {
        }

        internal static OrderItemsControllerBuilder Create()
        {
            return new OrderItemsControllerBuilder();
        }

        internal OrderItemsControllerBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        internal OrderItemsControllerBuilder WithUpdateOrderItemService(IUpdateOrderItemService updateOrderItemService)
        {
            _updateOrderItemRepository = updateOrderItemService;
            return this;
        }


        internal OrderItemsController Build() =>
            new OrderItemsController(_orderRepository, _updateOrderItemRepository);
    }
}
