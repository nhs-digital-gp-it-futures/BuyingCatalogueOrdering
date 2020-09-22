using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Kernel;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal class OrderCustomization : OrderingCustomization
    {
        public override void Customize(IFixture fixture)
        {
            base.Customize(fixture);

            fixture.Customize<Order>(c => c
                .FromFactory(new MethodInvoker(new FactoryMethodQuery()))
                .Do(o => o.AddOrderItem(
                    fixture.Create<OrderItem>(),
                    fixture.Create<Guid>(),
                    fixture.Create<string>()))
                .Do(o => o.SetServiceRecipients(
                    CreateServiceRecipients(fixture),
                    fixture.Create<Guid>(),
                    fixture.Create<string>())));
        }

        protected virtual IEnumerable<OdsOrganisation> CreateServiceRecipients(IFixture fixture)
        {
            return fixture.CreateMany<OdsOrganisation>();
        }
    }
}
