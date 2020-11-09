using AutoFixture.NUnit3;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    public sealed class OrderingInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public OrderingInlineAutoDataAttribute(params object[] arguments)
            : base(OrderingFixtureFactory.Create, arguments)
        {
        }
    }
}
