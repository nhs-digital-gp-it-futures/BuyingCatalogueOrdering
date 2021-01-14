using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ContactBuilder
    {
        private int contactId;
        private string firstName;
        private string lastName;
        private string email;
        private string phone;

        private ContactBuilder()
        {
            contactId = 123;
            firstName = "Adam";
            lastName = "Smith";
            email = "adminsmith@email.com";
            phone = "0123456789";
        }

        public static ContactBuilder Create() => new();

        public ContactBuilder WithContactId(int id)
        {
            contactId = id;
            return this;
        }

        public ContactBuilder WithFirstName(string name)
        {
            firstName = name;
            return this;
        }

        public ContactBuilder WithLastName(string name)
        {
            lastName = name;
            return this;
        }

        public ContactBuilder WithEmail(string address)
        {
            email = address;
            return this;
        }

        public ContactBuilder WithPhone(string number)
        {
            phone = number;
            return this;
        }

        public Contact Build()
        {
            return new()
            {
                ContactId = contactId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
            };
        }
    }
}
