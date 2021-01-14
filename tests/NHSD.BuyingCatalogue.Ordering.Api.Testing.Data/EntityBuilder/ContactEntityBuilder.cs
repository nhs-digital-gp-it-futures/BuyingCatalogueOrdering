using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class ContactEntityBuilder
    {
        private readonly ContactEntity contactEntity;

        public ContactEntityBuilder()
        {
            contactEntity = new ContactEntity();
        }

        public static ContactEntityBuilder Create()
        {
            return new();
        }

        public ContactEntityBuilder WithFirstName(string firstName)
        {
            contactEntity.FirstName = firstName;
            return this;
        }

        public ContactEntityBuilder WithLastName(string lastName)
        {
            contactEntity.LastName = lastName;
            return this;
        }

        public ContactEntityBuilder WithEmail(string email)
        {
            contactEntity.Email = email;
            return this;
        }

        public ContactEntityBuilder WithPhone(string phone)
        {
            contactEntity.Phone = phone;
            return this;
        }

        public ContactEntity Build()
        {
            return contactEntity;
        }
    }
}
