using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class SectionStatusControllerBuilder
    {
        private IOrderRepository _orderRepository;

        private SectionStatusControllerBuilder()
        {
        }

        internal static SectionStatusControllerBuilder Create() => new();

        internal SectionStatusControllerBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        internal SectionStatusController Build() => new(_orderRepository);
    }
}
