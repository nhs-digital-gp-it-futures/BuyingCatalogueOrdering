using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class PrimaryContactModelExtensions
    {
        internal static Contact ToObject(this PrimaryContactModel model)
        {
            if (model is null)
                return null;

            return new Contact
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.EmailAddress,
                Phone = model.TelephoneNumber
            };
        }
    }
}
