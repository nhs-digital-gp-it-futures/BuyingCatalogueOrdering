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


        [When(@"the user makes a request to update the order party on the order with the ID (.*)")]
        public async Task WhenTheUserMakesARequestToUpdateTheOrderPartyWithOrderId(string orderId, Table table)
        {
            var requestData = new OrganisationPartyPayload();
            var organisationData = table.CreateInstance<OrganisationPayload>();
            var organisationAddressData = table.CreateInstance<AddressPayload>();
            var contactData = table.CreateInstance<ContactPayload>();
            organisationData.address = organisationAddressData;
            requestData.PrimaryContact = contactData;
            requestData.Organisation = organisationData;
            await _request.PutJsonAsync(string.Format(_orderingPartyUrl, orderId), requestData);
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

        private sealed class OrganisationPartyPayload
        {
            public OrganisationPayload Organisation { get; set; }
            public ContactPayload PrimaryContact { get; set; }
        }

        private sealed class OrganisationPayload
        {
            public string Name { get; set; }
            public string OdsCode { get; set; }
            public AddressPayload address { get; set; }
        }

        private sealed class AddressPayload
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

        private sealed class ContactPayload
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public string TelephoneNumber { get; set; }
        }
    }
}
