using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class OrderCustomization : ICustomization
    {
#pragma warning disable CA5394 // Do not use insecure randomness
        public static readonly Random Random = new();

        public void Customize(IFixture fixture)
        {
            fixture.Customize<Order>(c => c.FromFactory(() => CreateOrder(fixture))
                .Without(o => o.CallOffId)
                .Without(o => o.Revision)
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

            var orderItems = fixture.CreateMany<OrderItem>(Random.Next(20, 40)).ToList();

            orderItems.ForEach(
                oi => oi.SetRecipients(fixture.CreateMany<OrderItemRecipient>(Random.Next(4, 9))));

            orderItems.ForEach(oi => order.AddOrUpdateOrderItem(oi));

            return order;
        }
#pragma warning restore CA5394 // Do not use insecure randomness
    }
}
