using System;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public static class OrderOrganisationId
    {
        public static Result<Guid> Create(Guid organisationId)
        {
            return organisationId == Guid.Empty
                ? Result.Failure<Guid>(OrderErrors.OrderOrganisationIdRequired())
                : Result.Success(organisationId);
        }
    }
}
