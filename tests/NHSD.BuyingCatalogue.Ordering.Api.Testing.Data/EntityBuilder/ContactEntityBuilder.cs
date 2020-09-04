using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class ContactEntityBuilder
    {
        private readonly ContactEntity _contactEntity;

        public ContactEntityBuilder()
        {
            _contactEntity = new ContactEntity();
        }

        public static ContactEntityBuilder Create()
        {
            return new ContactEntityBuilder();
        }

        public ContactEntityBuilder WithFirstName(string firstName)
        {
            _contactEntity.FirstName = firstName;
            return this;
        }

        public ContactEntityBuilder WithLastName(string lastName)
        {
            _contactEntity.LastName = lastName;
            return this;
        }

        public ContactEntityBuilder WithEmail(string email)
        {
            _contactEntity.Email = email;
            return this;
        }

        public ContactEntityBuilder WithPhone(string phone)
        {
            _contactEntity.Phone = phone;
            return this;
        }

        public ContactEntity Build()
        {
            return _contactEntity;
        }
    }
}
