using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class AddressBuilder
    {
        private readonly int _addressId;
        private readonly string _line1;
        private readonly string _line2;
        private readonly string _line3;
        private readonly string _line4;
        private readonly string _line5;
        private readonly string _town;
        private readonly string _county;
        private readonly string _postcode;
        private readonly string _country;

        private AddressBuilder()
        {
            _addressId = 321;
            _line1 = "Address line 1";
            _line2 = "Address line 2";
            _line3 = "Address line 3";
            _line4 = "Address line 4";
            _line5 = "Address line 5";
            _town = "Some town";
            _county = "Some county";
            _postcode = "Some postcode";
            _country = "Some country";
        }

        internal static AddressBuilder Create()
        {
            return new AddressBuilder();
        }

        internal Address Build()
        {
            return new Address
            {
                AddressId = _addressId,
                Line1 = _line1,
                Line2 = _line2,
                Line3 = _line3,
                Line4 = _line4,
                Line5 = _line5,
                Town = _town,
                County = _county,
                Postcode = _postcode,
                Country = _country
            };
        }
    }
}
