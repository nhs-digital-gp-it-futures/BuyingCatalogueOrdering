using System;
using System.Collections.Generic;
using System.Text;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    sealed public class OrderPartyContact : ValueObject
    {
        public Contact Value { get; }

        private OrderPartyContact (Contact value)
        {
            Value = value;
        }

        public static Result<OrderPartyContact> Create(string firstName, string lastName, string email, string phone)
        {
            var contact = new Contact
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone
            };
            return Result.Success(new OrderPartyContact(contact));
        }

        public Contact GetContact()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
