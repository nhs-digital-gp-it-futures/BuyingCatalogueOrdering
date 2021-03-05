using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.NUnit3;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture
{
    [SuppressMessage("Performance", "CA1813:Avoid unsealed attributes", Justification = "AutoFixture customization hierarchy")]
    public class CommonAutoDataAttribute : AutoDataAttribute
    {
        public CommonAutoDataAttribute()
            : base(FixtureFactory.Create)
        {
        }

        protected CommonAutoDataAttribute(params ICustomization[] customizations)
            : base(() => FixtureFactory.Create(customizations))
        {
        }
    }
}
