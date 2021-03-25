using System;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IIdentityService
    {
        Guid GetUserIdentity();

        string GetUserName();

        IIdentityUser GetUserInfo();
    }
}
