using System;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    public sealed class InMemoryDbFullOrderAutoDataAttribute : CommonAutoDataAttribute
    {
        public InMemoryDbFullOrderAutoDataAttribute()
            : base(
                new ControllerBaseCustomization(),
                new EnumValueByNameCustomization(),
                new OrderStatusCustomization(),
                new OrderItemsCustomization(),
                new InMemoryDbCustomization(Guid.NewGuid().ToString()))
        {
        }
    }
}
