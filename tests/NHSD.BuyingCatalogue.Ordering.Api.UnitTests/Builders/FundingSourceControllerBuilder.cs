using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class FundingSourceControllerBuilder
    {
        private IOrderRepository _orderRepository;

        private FundingSourceControllerBuilder() { }

        internal static FundingSourceControllerBuilder Create() { return new FundingSourceControllerBuilder();}

        internal FundingSourceControllerBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        internal FundingSourceController Build()
        {
            return new FundingSourceController(_orderRepository);
        }
    }
}
