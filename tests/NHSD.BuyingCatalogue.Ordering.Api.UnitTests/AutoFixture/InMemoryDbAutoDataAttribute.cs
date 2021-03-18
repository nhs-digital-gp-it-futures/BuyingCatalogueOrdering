using System;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    public sealed class InMemoryDbAutoDataAttribute : CommonAutoDataAttribute
    {
        public InMemoryDbAutoDataAttribute()
            : base(
                new ControllerBaseCustomization(),
                new EnumValueByNameCustomization(),
                new OrderCustomization(),
                new InMemoryDbCustomization(Guid.NewGuid().ToString()))
        {
        }
    }
}
