using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture
{
    internal static class FixtureFactory
    {
        private static readonly ICustomization[] Customizations =
        {
            new AutoMoqCustomization(),
            new CallOffIdCustomization(),
            new CatalogueItemIdCustomization(),
            new ErrorDetailsCustomization(),
            new OrderItemCustomization(),
        };

        internal static IFixture Create() => Create(Customizations);

        internal static IFixture Create(params ICustomization[] customizations) =>
            new Fixture().Customize(new CompositeCustomization(Customizations.Union(customizations)));
    }
}
