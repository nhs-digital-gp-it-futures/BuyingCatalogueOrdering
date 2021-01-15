using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class SingleServiceRecipientOrderCustomization : OrderCustomization
    {
        private readonly OdsOrganisation organization;

        public SingleServiceRecipientOrderCustomization(OdsOrganisation organization)
        {
            this.organization = organization;
        }

        public override void Customize(IFixture fixture)
        {
            fixture.Inject(organization);
            fixture.Customize<OrderItem>(c => new SingleServiceRecipientSpecimenBuilder(organization));

            base.Customize(fixture);
        }

        protected override IEnumerable<OdsOrganisation> CreateServiceRecipients(IFixture fixture)
        {
            return fixture.CreateMany<OdsOrganisation>(1);
        }

        private sealed class SingleServiceRecipientSpecimenBuilder : ISpecimenBuilder
        {
            private readonly OdsOrganisation organization;

            public SingleServiceRecipientSpecimenBuilder(OdsOrganisation organization)
            {
                this.organization = organization;
            }

            public object Create(object request, ISpecimenContext context)
            {
                if (request is not ParameterInfo pi)
                    return new NoSpecimen();

                if (IsOdsCodeParameter(pi))
                    return organization.Code;

                return new NoSpecimen();
            }

            private static bool IsOdsCodeParameter(ParameterInfo info)
            {
                return info.ParameterType == typeof(string) && info.Name == "odsCode";
            }
        }
    }
}
