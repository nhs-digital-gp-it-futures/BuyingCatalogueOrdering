using AutoFixture;
using AutoFixture.Kernel;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture
{
    internal sealed class ErrorDetailsCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<ErrorDetails>(c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));
        }
    }
}
