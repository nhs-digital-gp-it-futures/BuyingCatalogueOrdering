using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ContactBuilder
    {
        private int _contactId;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phone;

        private ContactBuilder()
        {
            _contactId = 123;
            _firstName = "Adam";
            _lastName = "Smith";
            _email = "adminsmith@email.com";
            _phone = "0123456789";
        }

        public static ContactBuilder Create()
        {
            return new ContactBuilder();
        }

        public ContactBuilder WithContactId(int contactId)
        {
            _contactId = contactId;
            return this;
        }

        public ContactBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public ContactBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public ContactBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public ContactBuilder WithPhone(string phone)
        {
            _phone = phone;
            return this;
        }

        public Contact Build()
        {
            return new Contact
            {
                ContactId = _contactId,
                FirstName = _firstName,
                LastName = _lastName,
                Email = _email,
                Phone = _phone
            };
        }
    }
}
