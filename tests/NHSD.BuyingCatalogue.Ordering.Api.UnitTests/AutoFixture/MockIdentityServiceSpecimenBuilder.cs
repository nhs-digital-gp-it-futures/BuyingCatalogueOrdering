using System;
using AutoFixture;
using AutoFixture.Kernel;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Application.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class MockIdentityServiceSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!(request as Type == typeof(IIdentityService)))
                return new NoSpecimen();

            var userId = context.Create<Guid>();
            var userName = context.Create<string>();

            var identityServiceMock = context.Create<Mock<IIdentityService>>();
            identityServiceMock.Setup(i => i.GetUserIdentity()).Returns(userId);
            identityServiceMock.Setup(i => i.GetUserName()).Returns(userName);
            identityServiceMock.Setup(i => i.GetUserInfo()).Returns(new IdentityUser(userId, userName));

            return identityServiceMock.Object;
        }
    }
}
