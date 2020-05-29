using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class AddressModelExtensions
    {
        internal static Address ToObject(this AddressModel model)
        {
            if (model is null)
                return null;

            return new Address
            {
                Line1 = model.Line1,
                Line2 = model.Line2,
                Line3 = model.Line3,
                Line4 = model.Line4,
                Line5 = model.Line5,
                Town = model.Town,
                County = model.County,
                Postcode = model.Postcode,
                Country = model.Country
            };
        }
    }
}
