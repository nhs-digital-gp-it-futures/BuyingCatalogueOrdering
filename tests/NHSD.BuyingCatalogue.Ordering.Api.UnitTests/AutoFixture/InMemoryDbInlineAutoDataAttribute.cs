using AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    public sealed class InMemoryDbInlineAutoDataAttribute : CommonInlineAutoDataAttribute
    {
        public InMemoryDbInlineAutoDataAttribute(string dbName, params object[] arguments)
            : base(GetCustomization(dbName), arguments)
        {
            DbName = dbName;
        }

        public string DbName { get; }

        private static ICustomization[] GetCustomization(string dbName)
        {
            return new ICustomization[]
            {
                new ControllerBaseCustomization(),
                new EnumValueByNameCustomization(),
                new OrderCustomization(),
                new InMemoryDbCustomization(dbName),
            };
        }
    }
}
