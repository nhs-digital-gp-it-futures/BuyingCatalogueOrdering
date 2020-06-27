using Moq;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Application.Services.CreateOrderItem;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class CreateOrderItemServiceBuilder
    {
        private IOrderRepository _orderRepository;
        private IIdentityService _identityService;

        private CreateOrderItemServiceBuilder()
        {
            _orderRepository = Mock.Of<IOrderRepository>();
            _identityService = Mock.Of<IIdentityService>();
        }

        public static CreateOrderItemServiceBuilder Create() => new CreateOrderItemServiceBuilder();

        public CreateOrderItemServiceBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        public CreateOrderItemServiceBuilder WithIdentityService(IIdentityService identityService)
        {
            _identityService = identityService;
            return this;
        }

        public CreateOrderItemService Build()
        {
            return new CreateOrderItemService(
                _orderRepository, 
                _identityService);
        }
    }
}
