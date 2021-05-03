using System;
using AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    public sealed class InMemoryDbInlineAutoDataAttribute : CommonInlineAutoDataAttribute
    {
        public InMemoryDbInlineAutoDataAttribute(params object[] arguments)
            : base(GetCustomization(), arguments)
        {
        }

        private static ICustomization[] GetCustomization()
        {
            return new ICustomization[]
            {
                new ControllerBaseCustomization(),
                new EnumValueByNameCustomization(),
                new OrderStatusCustomization(),
                new OrderCustomization(),
                new InMemoryDbCustomization(Guid.NewGuid().ToString()),
            };
        }
    }
}
