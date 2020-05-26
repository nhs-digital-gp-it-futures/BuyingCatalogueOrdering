using System;
using System.Collections.Generic;
using System.Text;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderPartyOrganisation : ValueObject
    {
        public Organisation Value { get; }

        private OrderPartyOrganisation(Organisation value)
        {
            Value = value;
        }

        public static Result<OrderPartyOrganisation> Create(string name, string odsCode, Address address)
        {
            var Organisation = new Organisation
            {
                Name = name,
                OdsCode = odsCode,
                Adress = address,
            };
            return Result.Success(new OrderPartyOrganisation(Organisation));
        }

        public Organisation GetOrganisation()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
