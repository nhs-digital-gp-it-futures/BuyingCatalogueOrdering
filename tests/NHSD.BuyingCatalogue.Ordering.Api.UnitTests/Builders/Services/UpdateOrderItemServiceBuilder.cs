using Moq;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Application.Services.UpdateOrderItem;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class UpdateOrderItemServiceBuilder
    {
        private IOrderRepository _orderRepository;
        private IIdentityService _identityService;

        private UpdateOrderItemServiceBuilder()
        {
            _orderRepository = Mock.Of<IOrderRepository>();
            _identityService = Mock.Of<IIdentityService>();
        }

        public static UpdateOrderItemServiceBuilder Create() => new UpdateOrderItemServiceBuilder();

        public UpdateOrderItemServiceBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        public UpdateOrderItemServiceBuilder WithIdentityService(IIdentityService identityService)
        {
            _identityService = identityService;
            return this;
        }

        public UpdateOrderItemService Build()
        {
            return new UpdateOrderItemService(
                _orderRepository, 
                _identityService);
        }
    }
}
