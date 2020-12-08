using System;
using AutoFixture.NUnit3;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    public sealed class OrderingAutoDataAttribute : AutoDataAttribute
    {
        public OrderingAutoDataAttribute()
            : base(OrderingFixtureFactory.Create)
        {
        }

        public OrderingAutoDataAttribute(string userId, string userName)
            : base(() => OrderingFixtureFactory.Create(Guid.Parse(userId), userName))
        {
        }
    }
}
