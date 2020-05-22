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

        public static Result<OrderPartyContact> Create(string firstName,string lastName,string email, string phone)
        {
            var firstNameIsValid = ContactName.Create(firstName, nameof(firstName));
            var lastNameIsValid = ContactName.Create(lastName, nameof(lastName));
            var emailIsValid = ContactEmail.Create(email);
            var phoneIsValid = ContactTelephone.Create(phone);

            if (!firstNameIsValid.IsSuccess || !lastNameIsValid.IsSuccess || !emailIsValid.IsSuccess || !phoneIsValid.IsSuccess)
            {
                var errors = new List<ErrorDetails>();
                errors.AddRange(firstNameIsValid.Errors);
                errors.AddRange(lastNameIsValid.Errors);
                errors.AddRange(emailIsValid.Errors);
                errors.AddRange(phoneIsValid.Errors);
                return Result.Failure<OrderPartyContact>(errors);
            }

            var contact = new Contact();
            contact.SetFirstName(firstNameIsValid.Value);
            contact.SetLastName(lastNameIsValid.Value);
            contact.SetEmail(emailIsValid.Value);
            contact.SetPhone(phoneIsValid.Value);
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
