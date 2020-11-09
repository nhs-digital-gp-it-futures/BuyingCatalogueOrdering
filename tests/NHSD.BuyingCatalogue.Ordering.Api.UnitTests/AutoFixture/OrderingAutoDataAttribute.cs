using AutoFixture.NUnit3;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    public sealed class OrderingAutoDataAttribute : AutoDataAttribute
    {
        public OrderingAutoDataAttribute()
            : base(OrderingFixtureFactory.Create)
        {
        }
    }
}
