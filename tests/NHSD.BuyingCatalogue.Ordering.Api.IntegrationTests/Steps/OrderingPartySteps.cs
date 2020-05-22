using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderingPartySteps
    {
        private readonly Response _response;
        private readonly Request _request;

        private readonly string _orderingPartyUrl;

        public OrderingPartySteps(Response response, Request request, Settings settings)
        {
            _response = response;
            _request = request;

            _orderingPartyUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/ordering-party";
        }


        [When(@"the user makes a request to retrieve the ordering-party section with the ID (.*)")]
        public async Task GivenTheUserMakesARequestToRetrieveTheOrdering_PartySectionWithTheID(string orderId)
        {
            await _request.GetAsync(string.Format(_orderingPartyUrl, orderId));
        }

        [Then(@"the ordering-party Organisation is returned")]
        public async Task ThenTheOrdering_PartyOrganisationIsReturned(Table table)
        {
            var expected = table.CreateSet<OrganisationTable>().FirstOrDefault();

            var response = (await _response.ReadBodyAsJsonAsync()).SelectToken("organisation");

            var actual = new OrganisationTable
            {
                Name = response.Value<string>("name"),
                OdsCode = response.Value<string>("odsCode")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        private sealed class OrganisationTable
        {
            public string Name { get; set; }
            public string OdsCode { get; set; }
        }
    }
}
