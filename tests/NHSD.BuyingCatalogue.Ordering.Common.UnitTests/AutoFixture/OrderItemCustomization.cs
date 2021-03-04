using System.Collections.Generic;
using AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture
{
    internal sealed class OrderItemCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<OrderItem>(
                c => c.Do(o => o.SetRecipients(fixture.Create<IEnumerable<OrderItemRecipient>>())));
        }
    }
}
