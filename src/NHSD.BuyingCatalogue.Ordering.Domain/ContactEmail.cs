using System.Collections.Generic;
using System.Linq;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ContactEmail : ValueObject
    {
        public string Value { get; }

        private ContactEmail()
        {

        }
        
        private ContactEmail(string value) : this()
        {
            Value = value;
        }

        public static Result<ContactEmail> Create(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return Result.Failure<ContactEmail>(OrderErrors.ContactEmailAddressRequired());
            }

            if (emailAddress.Length > 100)
            {
                return Result.Failure<ContactEmail>(OrderErrors.ContactEmailAddressTooLong());
            }

            if (emailAddress.Count(c=> c=='@')>1)
            {
                return Result.Failure<ContactEmail>(OrderErrors.ContactEmailAddressInvalidFormat());
            }

            return Result.Success(new ContactEmail(emailAddress));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
