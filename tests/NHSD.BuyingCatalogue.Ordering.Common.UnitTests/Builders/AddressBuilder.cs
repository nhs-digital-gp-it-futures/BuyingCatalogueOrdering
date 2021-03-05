using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders
{
    public sealed class AddressBuilder
    {
        private readonly int addressId;
        private string line1;
        private string line2;
        private string line3;
        private string line4;
        private string line5;
        private string town;
        private string county;
        private string postcode;
        private string country;

        private AddressBuilder()
        {
            addressId = 321;
            line1 = "Address line 1";
            line2 = "Address line 2";
            line3 = "Address line 3";
            line4 = "Address line 4";
            line5 = "Address line 5";
            town = "Some town";
            county = "Some county";
            postcode = "Some postcode";
            country = "Some country";
        }

        public static AddressBuilder Create() => new();

        public AddressBuilder WithLine1(string line)
        {
            line1 = line;
            return this;
        }

        public AddressBuilder WithLine2(string line)
        {
            line2 = line;
            return this;
        }

        public AddressBuilder WithLine3(string line)
        {
            line3 = line;
            return this;
        }

        public AddressBuilder WithLine4(string line)
        {
            line4 = line;
            return this;
        }

        public AddressBuilder WithLine5(string line)
        {
            line5 = line;
            return this;
        }

        public AddressBuilder WithTown(string value)
        {
            town = value;
            return this;
        }

        public AddressBuilder WithCounty(string value)
        {
            county = value;
            return this;
        }

        public AddressBuilder WithPostcode(string value)
        {
            postcode = value;
            return this;
        }

        public AddressBuilder WithCountry(string value)
        {
            country = value;
            return this;
        }

        public Address Build()
        {
            return new()
            {
                Id = addressId,
                Line1 = line1,
                Line2 = line2,
                Line3 = line3,
                Line4 = line4,
                Line5 = line5,
                Town = town,
                County = county,
                Postcode = postcode,
                Country = country,
            };
        }
    }
}
