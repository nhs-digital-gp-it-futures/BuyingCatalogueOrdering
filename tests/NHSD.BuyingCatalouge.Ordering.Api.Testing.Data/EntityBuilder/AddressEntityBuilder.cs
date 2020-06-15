using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class AddressEntityBuilder
    {
        private readonly AddressEntity _addressEntity;

        public AddressEntityBuilder()
        {
            _addressEntity = new AddressEntity();
        }

        public static AddressEntityBuilder Create()
        {
            return new AddressEntityBuilder();
        }

        public AddressEntityBuilder WithLine1(string line1)
        {
            _addressEntity.Line1 = line1;
            return this;
        }

        public AddressEntityBuilder WithLine2(string line2)
        {
            _addressEntity.Line2 = line2;
            return this;
        }

        public AddressEntityBuilder WithLine3(string line3)
        {
            _addressEntity.Line3 = line3;
            return this;
        }

        public AddressEntityBuilder WithLine4(string line4)
        {
            _addressEntity.Line4 = line4;
            return this;
        }

        public AddressEntityBuilder WithLine5(string line5)
        {
            _addressEntity.Line5 = line5;
            return this;
        }

        public AddressEntityBuilder WithTown(string town)
        {
            _addressEntity.Town = town;
            return this;
        }

        public AddressEntityBuilder WithCounty(string county)
        {
            _addressEntity.County = county;
            return this;
        }

        public AddressEntityBuilder WithPostcode(string postcode)
        {
            _addressEntity.Postcode = postcode;
            return this;
        }

        public AddressEntityBuilder WithCountry(string country)
        {
            _addressEntity.Country = country;
            return this;
        }

        public AddressEntity Build()
        {
            return _addressEntity;
        }
    }
}
