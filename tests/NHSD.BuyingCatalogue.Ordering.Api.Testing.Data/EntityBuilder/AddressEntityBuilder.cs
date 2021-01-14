using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class AddressEntityBuilder
    {
        private readonly AddressEntity addressEntity;

        public AddressEntityBuilder()
        {
            addressEntity = new AddressEntity();
        }

        public static AddressEntityBuilder Create()
        {
            return new();
        }

        public AddressEntityBuilder WithLine1(string line1)
        {
            addressEntity.Line1 = line1;
            return this;
        }

        public AddressEntityBuilder WithLine2(string line2)
        {
            addressEntity.Line2 = line2;
            return this;
        }

        public AddressEntityBuilder WithLine3(string line3)
        {
            addressEntity.Line3 = line3;
            return this;
        }

        public AddressEntityBuilder WithLine4(string line4)
        {
            addressEntity.Line4 = line4;
            return this;
        }

        public AddressEntityBuilder WithLine5(string line5)
        {
            addressEntity.Line5 = line5;
            return this;
        }

        public AddressEntityBuilder WithTown(string town)
        {
            addressEntity.Town = town;
            return this;
        }

        public AddressEntityBuilder WithCounty(string county)
        {
            addressEntity.County = county;
            return this;
        }

        public AddressEntityBuilder WithPostcode(string postcode)
        {
            addressEntity.Postcode = postcode;
            return this;
        }

        public AddressEntityBuilder WithCountry(string country)
        {
            addressEntity.Country = country;
            return this;
        }

        public AddressEntity Build()
        {
            return addressEntity;
        }
    }
}
