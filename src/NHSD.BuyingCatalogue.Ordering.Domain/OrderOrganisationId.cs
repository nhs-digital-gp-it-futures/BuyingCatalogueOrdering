using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderOrganisationId : ValueObject
    {
        public Guid Value { get; }

        private OrderOrganisationId()
        {
        }

        private OrderOrganisationId(Guid value) : this()
        {
            Value = value;
        }

        public static Result<Guid> Create(Guid organisationId)
        {
            if (organisationId==Guid.Empty)
            {
                return Result.Failure<Guid>(OrderErrors.OrderOrganisationIdRequired());
            }
            return Result.Success(organisationId);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
