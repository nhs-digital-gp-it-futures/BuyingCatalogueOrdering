using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class AddressSteps
    {
        private readonly ScenarioContext _context;
        private readonly Response _response;
        private readonly Settings _settings;

        public AddressSteps(ScenarioContext context, Response response, Settings settings)
        {
            _context = context;
            _response = response;
            _settings = settings;
        }

        [Given(@"Addresses exist")]
        public async Task GivenAddressesExist(Table table)
        {
            IDictionary<string, int> addressDictionary = new Dictionary<string, int>();

            foreach (var addressItem in table.CreateSet<AddressTable>())
            {
                var address = AddressEntityBuilder
                    .Create()
                    .WithLine1(addressItem.Line1)
                    .WithLine2(addressItem.Line2)
                    .WithLine3(addressItem.Line3)
                    .WithLine4(addressItem.Line4)
                    .WithLine5(addressItem.Line5)
                    .WithTown(addressItem.Town)
                    .WithCounty(addressItem.County)
                    .WithPostcode(addressItem.Postcode)
                    .WithCountry(addressItem.Country)
                    .Build();

                var addressId = await address.InsertAsync<int>(_settings.ConnectionString);
                addressDictionary.Add(address.Postcode, addressId);
            }

            _context[ScenarioContextKeys.AddressMapDictionary] = addressDictionary;
        }

        [Then(@"the Address section is returned")]
        public async Task ThenTheAddressSectionIsReturned(Table table)
        {
            var expected = table.CreateSet<AddressTable>().FirstOrDefault();
            var response = (await _response.ReadBodyAsJsonAsync()).SelectToken("address");

            var actual = new AddressTable
            {
                Line1 = response.Value<string>("line1"),
                Line2 = response.Value<string>("line2"),
                Line3 = response.Value<string>("line3"),
                Line4 = response.Value<string>("line4"),
                Line5 = response.Value<string>("line5"),
                Town = response.Value<string>("town"),
                County = response.Value<string>("county"),
                Postcode = response.Value<string>("postcode"),
                Country = response.Value<string>("country")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Given(@"the user wants to update the (.*) section for the address")]
        public void WhenTheUserWantsToUpdateTheSupplierAddressSectionForTheAddress(string section, Table table)
        {
            var address = table.CreateSet<AddressTable>().FirstOrDefault();
            _context[section] = address;
        }

        private sealed class AddressTable
        {
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string Line3 { get; set; }
            public string Line4 { get; set; }
            public string Line5 { get; set; }
            public string Town { get; set; }
            public string County { get; set; }
            public string Postcode { get; set; }
            public string Country { get; set; }
        }
    }
}
