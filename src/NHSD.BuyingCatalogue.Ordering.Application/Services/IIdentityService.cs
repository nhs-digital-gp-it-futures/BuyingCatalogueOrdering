using System;

namespace NHSD.BuyingCatalogue.Ordering.Application.Services
{
    public interface IIdentityService
    {
        Guid GetUserIdentity();

        string GetUserName();

        IdentityUser GetUserInfo();
    }
}
