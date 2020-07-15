using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders
{
    public sealed class AddressBuilder
    {
        private readonly int _addressId;
        private string _line1;
        private string _line2;
        private string _line3;
        private string _line4;
        private string _line5;
        private string _town;
        private string _county;
        private string _postcode;
        private string _country;

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

        public static AddressBuilder Create()
        {
            return new AddressBuilder();
        }

        public AddressBuilder WithLine1(string line1)
        {
            _line1 = line1;
            return this;
        }

        public AddressBuilder WithLine2(string line2)
        {
            _line2 = line2;
            return this;
        }

        public AddressBuilder WithLine3(string line3)
        {
            _line3 = line3;
            return this;
        }

        public AddressBuilder WithLine4(string line4)
        {
            _line4 = line4;
            return this;
        }

        public AddressBuilder WithLine5(string line5)
        {
            _line5 = line5;
            return this;
        }

        public AddressBuilder WithTown(string town)
        {
            _town = town;
            return this;
        }

        public AddressBuilder WithCounty(string county)
        {
            _county = county;
            return this;
        }

        public AddressBuilder WithPostcode(string postcode)
        {
            _postcode = postcode;
            return this;
        }

        public AddressBuilder WithCountry(string country)
        {
            _country = country;
            return this;
        }

        public Address Build()
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
