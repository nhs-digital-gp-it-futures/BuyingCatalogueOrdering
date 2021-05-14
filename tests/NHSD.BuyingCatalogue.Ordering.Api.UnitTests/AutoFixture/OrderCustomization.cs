using AutoFixture;
using AutoFixture.Kernel;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class OrderCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Order>(c => c.FromFactory(() => CreateOrder(fixture))
                .Without(o => o.CallOffId)
                .Without(o => o.Revision)
                .Without(o => o.IsDeleted)
                .Without(o => o.OrderStatus));
        }

        private static Order CreateOrder(ISpecimenBuilder fixture)
        {
            var callOffId = fixture.Create<CallOffId>();
            var order = new Order
            {
                CallOffId = callOffId,
                Revision = callOffId.Revision,
            };

            BackingField.SetValue(order, nameof(Order.Id), callOffId.Id);

            return order;
        }
    }
}
