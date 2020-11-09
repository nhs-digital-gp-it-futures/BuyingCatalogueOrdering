using System;
using AutoFixture;
using AutoFixture.Kernel;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal class OrderingCustomization : ICustomization
    {
        public virtual void Customize(IFixture fixture)
        {
            fixture.Register<string, OrderDescription>(s => OrderDescription.Create(s).Value);

            fixture.Customize<ErrorDetails>(c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));
            fixture.Customize<GetOrderItemModel>(c => c.OmitAutoProperties());
            fixture.Customize<TimeUnitModel>(c => c.FromFactory(() => CreateTimeUnitModel(fixture)).OmitAutoProperties());

            fixture.Customize<CreateOrderItemService>(c => new MockIdentityServiceSpecimenBuilder());
            fixture.Customize<CreateOrderItemService>(c => new MockCreateOrderItemValidatorSpecimenBuilder());
        }

        private static TimeUnitModel CreateTimeUnitModel(ISpecimenBuilder fixture)
        {
            var timeUnit = fixture.Create<TimeUnit>();

            return new TimeUnitModel
            {
                Description = timeUnit.Description(),
                Name = timeUnit.Name(),
            };
        }

        private sealed class MockIdentityServiceSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(IIdentityService)))
                    return new NoSpecimen();

                var identityServiceMock = context.Create<Mock<IIdentityService>>();
                identityServiceMock.Setup(i => i.GetUserIdentity()).Returns(context.Create<Guid>());
                identityServiceMock.Setup(i => i.GetUserName()).Returns(context.Create<string>());

                return identityServiceMock.Object;
            }
        }

        private sealed class MockCreateOrderItemValidatorSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(ICreateOrderItemValidator)))
                    return new NoSpecimen();

                var validatorMock = context.Create<Mock<ICreateOrderItemValidator>>();
                validatorMock.Setup(v => v.Validate(It.IsNotNull<CreateOrderItemRequest>()))
                    .Returns(new ValidationResult(Array.Empty<ErrorDetails>()));

                return validatorMock.Object;
            }
        }
    }
}
