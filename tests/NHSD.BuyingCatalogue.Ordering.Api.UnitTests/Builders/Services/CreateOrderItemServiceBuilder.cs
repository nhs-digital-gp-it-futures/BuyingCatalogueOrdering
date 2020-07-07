using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class CreateOrderItemServiceBuilder
    {
        private IOrderRepository _orderRepository;
        private IIdentityService _identityService;
        private ICreateOrderItemValidator _createOrderItemValidator;

        private CreateOrderItemServiceBuilder()
        {
            _orderRepository = Mock.Of<IOrderRepository>();
            _identityService = Mock.Of<IIdentityService>();
            _createOrderItemValidator = Mock.Of<ICreateOrderItemValidator>();
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

        public CreateOrderItemServiceBuilder WithValidator(ICreateOrderItemValidator createOrderItemValidator)
        {
            _createOrderItemValidator = createOrderItemValidator;
            return this;
        }

        public CreateOrderItemService Build()
        {
            return new CreateOrderItemService(
                _orderRepository, 
                _identityService,
                _createOrderItemValidator);
        }
    }
}
