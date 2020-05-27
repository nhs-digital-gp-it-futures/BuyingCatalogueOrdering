using System;
using System.Collections.Generic;
using System.Text;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public static class OrderExtensions
    {
        public static bool IsContactComplete(this Contact contact)
        {
            if (contact == null)
            {
                return false;
            }
            else
            {
                return (!string.IsNullOrEmpty(contact.FirstName) &&
                    !string.IsNullOrEmpty(contact.LastName) &&
                    !string.IsNullOrEmpty(contact.Email) &&
                    !string.IsNullOrEmpty(contact.Phone));
            }
        }
        public static bool IsAddressComplete(this Address address)
        {
            if (address == null)
            {
                return false;
            }
            else
            {
                return (!string.IsNullOrEmpty(address.Line1) &&
                    !string.IsNullOrEmpty(address.Town) &&
                    !string.IsNullOrEmpty(address.Postcode));
            }
        }

        public static bool IsOrderPartyComplete(this Order order)
        {
            if (order == null)
            {
                return false;
            }
            else
            {
                return (!string.IsNullOrEmpty(order.OrganisationName) &&
                    !string.IsNullOrEmpty(order.OrganisationOdsCode) &&
                    order.OrganisationAddress.IsAddressComplete() &&
                    order.OrganisationContact.IsContactComplete());
            }
        }
    }
}
