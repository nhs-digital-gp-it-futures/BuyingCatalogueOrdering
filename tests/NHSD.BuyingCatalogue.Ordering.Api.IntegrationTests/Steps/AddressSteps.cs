using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class AddressSteps
    {
        private readonly Response response;
        private readonly Settings settings;
        private readonly OrderContext orderContext;

        public AddressSteps(Response response, Settings settings, OrderContext orderContext)
        {
            this.response = response;
            this.settings = settings;
            this.orderContext = orderContext;
        }

        [Given(@"Addresses exist")]
        public async Task GivenAddressesExist(Table table)
        {
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

                var addressId = await address.InsertAsync<int>(settings.ConnectionString);
                address.AddressId = addressId;

                orderContext.AddressReferenceList.Add(address);
            }
        }

        [Then(@"the Address is returned")]
        public async Task ThenTheAddressSectionIsReturned(Table table)
        {
            var expected = table.CreateSet<AddressTable>().FirstOrDefault();
            var address = (await response.ReadBodyAsJsonAsync()).SelectToken("address");

            var actual = new AddressTable
            {
                Line1 = address.Value<string>("line1"),
                Line2 = address.Value<string>("line2"),
                Line3 = address.Value<string>("line3"),
                Line4 = address.Value<string>("line4"),
                Line5 = address.Value<string>("line5"),
                Town = address.Value<string>("town"),
                County = address.Value<string>("county"),
                Postcode = address.Value<string>("postcode"),
                Country = address.Value<string>("country"),
            };

            actual.Should().BeEquivalentTo(expected);
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
