using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class FundingSourceControllerBuilder
    {
        private IOrderRepository _orderRepository;
        private ControllerContext _controllerContext;

        public FundingSourceControllerBuilder()
        {
            _orderRepository = Mock.Of<IOrderRepository>();
            _controllerContext = null;
        }

        public static FundingSourceControllerBuilder Create() => 
            new FundingSourceControllerBuilder();

        public FundingSourceControllerBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        public FundingSourceControllerBuilder WithControllerContext(ControllerContext controllerContext)
        {
            _controllerContext = controllerContext;
            return this;
        }

        public FundingSourceController Build() => 
            new FundingSourceController(_orderRepository)
            {
                ControllerContext = _controllerContext
            };
    }
}
