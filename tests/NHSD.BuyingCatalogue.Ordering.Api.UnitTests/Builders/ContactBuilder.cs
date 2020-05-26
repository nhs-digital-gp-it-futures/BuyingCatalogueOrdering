using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ContactBuilder
    {
        private readonly int _contactId;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _email;
        private readonly string _phone;

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
