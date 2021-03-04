using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    public sealed class InMemoryDbAutoDataAttribute : CommonAutoDataAttribute
    {
        public InMemoryDbAutoDataAttribute(string dbName)
            : base(
                new ControllerBaseCustomization(),
                new EnumValueByNameCustomization(),
                new OrderCustomization(),
                new InMemoryDbCustomization(dbName))
        {
            DbName = dbName;
        }

        public string DbName { get; }
    }
}
