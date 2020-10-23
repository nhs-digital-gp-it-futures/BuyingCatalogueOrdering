using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class CreateOrderItemServiceBuilder
    {
        private IOrderRepository orderRepository;
        private IIdentityService identityService;
        private ICreateOrderItemValidator createOrderItemValidator;

        private CreateOrderItemServiceBuilder()
        {
            orderRepository = Mock.Of<IOrderRepository>();
            identityService = Mock.Of<IIdentityService>();
            createOrderItemValidator = Mock.Of<ICreateOrderItemValidator>();
        }

        public static CreateOrderItemServiceBuilder Create() => new CreateOrderItemServiceBuilder();

        public CreateOrderItemServiceBuilder WithOrderRepository(IOrderRepository repository)
        {
            this.orderRepository = repository;
            return this;
        }

        public CreateOrderItemServiceBuilder WithIdentityService(IIdentityService service)
        {
            this.identityService = service;
            return this;
        }

        public CreateOrderItemServiceBuilder WithValidator(ICreateOrderItemValidator validator)
        {
            this.createOrderItemValidator = validator;
            return this;
        }

        public CreateOrderItemService Build()
        {
            return new CreateOrderItemService(
                orderRepository,
                identityService,
                Mock.Of<IOrderItemFactory>(),
                createOrderItemValidator);
        }
    }
}
