using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class AddressExtensions
    {
        internal static AddressModel ToModel(this Address address)
        {
            if (address is null)
                return null;

            return new AddressModel
            {
                Line1 = address.Line1,
                Line2 = address.Line2,
                Line3 = address.Line3,
                Line4 = address.Line4,
                Line5 = address.Line5,
                Town = address.Town,
                County = address.County,
                Postcode = address.Postcode,
                Country = address.Country,
            };
        }

        internal static Address ToDomain(this AddressModel model)
        {
            return model is null
                ? null
                : new Address
                {
                    Line1 = model.Line1,
                    Line2 = model.Line2,
                    Line3 = model.Line3,
                    Line4 = model.Line4,
                    Line5 = model.Line5,
                    Town = model.Town,
                    County = model.County,
                    Postcode = model.Postcode,
                    Country = model.Country,
                };
        }
    }
}
