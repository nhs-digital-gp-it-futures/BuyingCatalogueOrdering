using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    public sealed class OrderingAutoDataAttribute : AutoDataAttribute
    {
        public OrderingAutoDataAttribute()
            : base(() => new Fixture().Customize(new CompositeCustomization(
                new AutoMoqCustomization(),
                new ControllerBaseCustomization(),
                new OrderingCustomization(),
                new EnumValueByNameCustomization())))
        {
        }
    }
}
