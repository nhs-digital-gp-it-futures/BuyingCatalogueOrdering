using AutoFixture;
using AutoFixture.AutoMoq;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal static class OrderingFixtureFactory
    {
        private static readonly ICustomization[] Customizations =
        {
            new AutoMoqCustomization(),
            new ControllerBaseCustomization(),
            new OrderingCustomization(),
            new EnumValueByNameCustomization(),
        };

        private static ICustomization Instance { get; } = new CompositeCustomization(Customizations);

        internal static IFixture Create() => new Fixture().Customize(Instance);
    }
}
