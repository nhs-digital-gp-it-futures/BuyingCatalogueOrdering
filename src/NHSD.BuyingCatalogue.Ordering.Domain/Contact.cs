using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Contact
    {
        public int ContactId { get; set; }
        public ContactName FirstName { get;  private set; }
        public ContactName LastName { get; private set; }
        public ContactEmail Email { get; private set; }
        public ContactTelephone Phone { get; private set; }

        public void SetFirstName(ContactName firstName)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        }

        public void SetLastName(ContactName lastName)
        {
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        }

        public void SetEmail(ContactEmail email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        public void SetPhone(ContactTelephone phone)
        {
            phone = phone ?? throw new ArgumentNullException(nameof(phone));
        }

    }
}
