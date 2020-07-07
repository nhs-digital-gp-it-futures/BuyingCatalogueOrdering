using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class SectionStatusControllerBuilder
    {
        private IOrderRepository _orderRepository;

        private SectionStatusControllerBuilder()
        {
        }

        internal static SectionStatusControllerBuilder Create()
        {
            return new SectionStatusControllerBuilder();
        }

        internal SectionStatusControllerBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        internal SectionStatusController Build()
        {
            return new SectionStatusController(_orderRepository);
        }
    }
}
