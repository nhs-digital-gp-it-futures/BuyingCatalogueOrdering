using AutoFixture;
using AutoFixture.Kernel;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal class OrderingCustomization : ICustomization
    {
        public virtual void Customize(IFixture fixture)
        {
            fixture.Register<string, OrderDescription>(s => OrderDescription.Create(s).Value);
            fixture.Register(() => CatalogueItemType.Solution);

            fixture.Customize<TimeUnitModel>(c => c.FromFactory(() => CreateTimeUnitModel(fixture)).OmitAutoProperties());
            fixture.Customize<GetOrderItemModel>(c => c.OmitAutoProperties());
        }

        private static TimeUnitModel CreateTimeUnitModel(ISpecimenBuilder fixture)
        {
            var timeUnit = fixture.Create<TimeUnit>();

            return new TimeUnitModel
            {
                Description = timeUnit.Description(),
                Name = timeUnit.Name(),
            };
        }
    }
}
