using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.NUnit3;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture
{
    [SuppressMessage("Performance", "CA1813:Avoid unsealed attributes", Justification = "AutoFixture customization hierarchy")]
    public class CommonInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public CommonInlineAutoDataAttribute(params object[] arguments)
            : base(FixtureFactory.Create, arguments)
        {
        }

        protected CommonInlineAutoDataAttribute(ICustomization[] customizations, params object[] arguments)
            : base(() => FixtureFactory.Create(customizations), arguments)
        {
        }
    }
}
