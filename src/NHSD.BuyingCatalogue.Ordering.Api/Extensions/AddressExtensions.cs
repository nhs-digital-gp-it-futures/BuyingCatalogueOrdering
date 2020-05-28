﻿using NHSD.BuyingCatalogue.Ordering.Api.Models;
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
                Country = address.Country
            };
        }
    }
}