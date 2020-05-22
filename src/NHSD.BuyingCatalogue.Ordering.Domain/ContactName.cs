using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ContactName : ValueObject
    {
        public string Value { get; }

        private ContactName()
        {

        }

        private ContactName(string value) : this()
        {
            Value = value;
        }

        public static Result<ContactName> Create(string name , string fieldName ="Name")
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure<ContactName>(OrderErrors.ContactNameRequired(fieldName));
            }

            if (name.Length > 100)
            {
                return Result.Failure<ContactName>(OrderErrors.ContactNameRequired(fieldName));
            }

            return Result.Success(new ContactName(name));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
