using System;
using AutoFixture;
using AutoFixture.Kernel;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture
{
    internal sealed class CallOffIdCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CallOffId>(_ => new CallOffIdSpecimenBuilder());
        }

        private sealed class CallOffIdSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(CallOffId)))
                    return new NoSpecimen();

                var id = (context.Create<int>() % CallOffId.MaxId) + 1;
                var revision = (context.Create<byte>() % CallOffId.MaxRevision) + 1;

                return new CallOffId(id, (byte)revision);
            }
        }
    }
}
