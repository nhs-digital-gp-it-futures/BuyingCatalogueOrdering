using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services
{
    public interface IContactDetailsService
    {
        Address AddOrUpdateAddress(Address existingAddress, AddressModel newOrUpdatedAddress);

        Contact AddOrUpdatePrimaryContact(Contact existingContact, PrimaryContactModel newOrUpdatedContact);
    }
}
