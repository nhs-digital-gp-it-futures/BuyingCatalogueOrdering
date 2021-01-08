using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class UpdateOrderItemServiceBuilder
    {
        private IOrderRepository _orderRepository;
        private IIdentityService _identityService;
        private IUpdateOrderItemValidator _orderItemValidator;

        private UpdateOrderItemServiceBuilder()
        {
            _orderRepository = Mock.Of<IOrderRepository>();
            _identityService = Mock.Of<IIdentityService>();
            _orderItemValidator = Mock.Of<IUpdateOrderItemValidator>();
        }

        public static UpdateOrderItemServiceBuilder Create() => new();

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

        public UpdateOrderItemServiceBuilder WithOrderItemValidator(IUpdateOrderItemValidator orderItemValidator)
        {
            _orderItemValidator = orderItemValidator;
            return this;
        }

        public UpdateOrderItemService Build()
        {
            return new(
                _orderRepository,
                _identityService,
                _orderItemValidator);
        }
    }
}
