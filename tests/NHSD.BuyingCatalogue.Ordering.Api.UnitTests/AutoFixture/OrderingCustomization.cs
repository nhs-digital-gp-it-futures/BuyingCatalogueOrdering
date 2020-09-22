using AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal class OrderingCustomization : ICustomization
    {
        private readonly OrderingCustomizationOptions options;

        internal OrderingCustomization()
            : this(new OrderingCustomizationOptions())
        {
        }

        internal OrderingCustomization(OrderingCustomizationOptions options)
        {
            this.options = options;
        }

        public virtual void Customize(IFixture fixture)
        {
            fixture.Register<string, OrderDescription>(s => OrderDescription.Create(s).Value);
            fixture.Register(() => CatalogueItemType.Solution);
            fixture.Register(() => CataloguePriceType.Flat);

            fixture.Register(() => options.ProvisioningType ?? ProvisioningType.OnDemand);
            fixture.Register(() => options.TimeUnit);
        }
    }
}
