using System.Collections.Generic;
using System.Linq;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ContactTelephone : ValueObject
    {
        public string Value { get; }

        private ContactTelephone()
        {

        }
        
        private ContactTelephone(string value) : this()
        {
            Value = value;
        }

        public static Result<ContactTelephone> Create(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return Result.Failure<ContactTelephone>(OrderErrors.ContactTelephoneNumberRequired());
            }

            if (emailAddress.Length > 35)
            {
                return Result.Failure<ContactTelephone>(OrderErrors.ContactTelephoneNumberTooLong());
            }

            return Result.Success(new ContactTelephone(emailAddress));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
