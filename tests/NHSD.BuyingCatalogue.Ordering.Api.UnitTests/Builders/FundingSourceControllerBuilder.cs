using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class FundingSourceControllerBuilder
    {
        private IOrderRepository orderRepository;
        private ControllerContext controllerContext;

        public FundingSourceControllerBuilder()
        {
            orderRepository = Mock.Of<IOrderRepository>();
            controllerContext = null;
        }

        public static FundingSourceControllerBuilder Create() => new();

        public FundingSourceControllerBuilder WithOrderRepository(IOrderRepository repository)
        {
            orderRepository = repository;
            return this;
        }

        public FundingSourceControllerBuilder WithControllerContext(ControllerContext context)
        {
            controllerContext = context;
            return this;
        }

        public FundingSourceController Build() => new(orderRepository) { ControllerContext = controllerContext };
    }
}
