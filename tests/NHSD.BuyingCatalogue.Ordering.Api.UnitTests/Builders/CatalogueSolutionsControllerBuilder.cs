using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Services.UpdateOrderItem;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class CatalogueSolutionsControllerBuilder
    {
        private IOrderRepository _orderRepository;
        private ICreateOrderItemService _createOrderItemService;
        private IUpdateOrderItemService _updateOrderItemService;

        private CatalogueSolutionsControllerBuilder()
        {
        }

        internal static CatalogueSolutionsControllerBuilder Create()
        {
            return new CatalogueSolutionsControllerBuilder();
        }

        internal CatalogueSolutionsControllerBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        internal CatalogueSolutionsControllerBuilder WithCreateOrderItemService(ICreateOrderItemService createOrderItemService)
        {
            _createOrderItemService = createOrderItemService;
            return this;
        }

        internal CatalogueSolutionsControllerBuilder WithUpdateOrderItemService(IUpdateOrderItemService updateOrderItemService)
        {
            _updateOrderItemService = updateOrderItemService;
            return this;
        }

        internal CatalogueSolutionsController Build() => 
            new CatalogueSolutionsController(
                _orderRepository, 
                _createOrderItemService, 
                _updateOrderItemService);
    }
}
