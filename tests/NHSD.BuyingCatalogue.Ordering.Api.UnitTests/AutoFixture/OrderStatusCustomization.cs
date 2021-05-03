using System;
using AutoFixture;
using AutoFixture.Kernel;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class OrderStatusCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<OrderStatus>(_ => new OrderStatusSpecimenBuilder());
        }

        private sealed class OrderStatusSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(OrderStatus)))
                {
                    return new NoSpecimen();
                }

                return DateTime.Now.Ticks % 2 == 0 ? OrderStatus.Complete : OrderStatus.Incomplete;
            }
        }
    }
}
