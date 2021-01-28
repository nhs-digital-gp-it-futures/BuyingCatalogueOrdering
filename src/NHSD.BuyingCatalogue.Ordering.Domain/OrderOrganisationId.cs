using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderOrganisationId : ValueObject
    {
        private OrderOrganisationId()
        {
        }

        private OrderOrganisationId(Guid value)
            : this()
        {
            Value = value;
        }

        public Guid Value { get; }

        public static Result<Guid> Create(Guid organisationId)
        {
            return organisationId == Guid.Empty
                ? Result.Failure<Guid>(OrderErrors.OrderOrganisationIdRequired())
                : Result.Success(organisationId);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
