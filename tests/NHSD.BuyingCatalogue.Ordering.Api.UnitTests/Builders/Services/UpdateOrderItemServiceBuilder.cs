using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class UpdateOrderItemServiceBuilder
    {
        private IOrderRepository orderRepository;
        private IIdentityService identityService;
        private IUpdateOrderItemValidator orderItemValidator;

        private UpdateOrderItemServiceBuilder()
        {
            orderRepository = Mock.Of<IOrderRepository>();
            identityService = Mock.Of<IIdentityService>();
            orderItemValidator = Mock.Of<IUpdateOrderItemValidator>();
        }

        public static UpdateOrderItemServiceBuilder Create() => new();

        public UpdateOrderItemServiceBuilder WithOrderRepository(IOrderRepository repository)
        {
            orderRepository = repository;
            return this;
        }

        public UpdateOrderItemServiceBuilder WithIdentityService(IIdentityService service)
        {
            identityService = service;
            return this;
        }

        public UpdateOrderItemServiceBuilder WithOrderItemValidator(IUpdateOrderItemValidator validator)
        {
            orderItemValidator = validator;
            return this;
        }

        public UpdateOrderItemService Build()
        {
            return new(
                orderRepository,
                identityService,
                orderItemValidator);
        }
    }
}
