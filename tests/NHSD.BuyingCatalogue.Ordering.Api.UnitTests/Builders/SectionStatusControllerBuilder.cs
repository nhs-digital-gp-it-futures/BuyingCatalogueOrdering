using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class SectionStatusControllerBuilder
    {
        private IOrderRepository orderRepository;

        private SectionStatusControllerBuilder()
        {
        }

        internal static SectionStatusControllerBuilder Create() => new();

        internal SectionStatusControllerBuilder WithOrderRepository(IOrderRepository repository)
        {
            orderRepository = repository;
            return this;
        }

        internal SectionStatusController Build() => new(orderRepository);
    }
}
