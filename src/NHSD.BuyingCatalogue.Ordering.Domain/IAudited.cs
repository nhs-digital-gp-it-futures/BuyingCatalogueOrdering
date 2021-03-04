using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public interface IAudited
    {
        void SetLastUpdatedBy(Guid userId, string userName);
    }
}
