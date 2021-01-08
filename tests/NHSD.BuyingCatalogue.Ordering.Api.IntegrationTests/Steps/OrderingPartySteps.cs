using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
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
        private readonly ScenarioContext _context;

        public OrderingPartySteps(Response response, Request request, Settings settings, ScenarioContext context)
        {
            _response = response;
            _request = request;
            _context = context;
            _orderingPartyUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/ordering-party";
        }

        private static OrganisationPartyPayload GetOrganisationPartyPayloadByOrderId(ScenarioContext context, string orderId)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (orderId == null)
                return null;

            var payloadDictionary =
                context.Get<IDictionary<string, OrganisationPartyPayload>>(ScenarioContextKeys.OrganisationPayloadDictionary, new Dictionary<string, OrganisationPartyPayload>());

            if (payloadDictionary.TryGetValue(orderId, out var payload))
                return payload;

            return null;
        }

        private static void SetOrganisationPartyPayloadByOrderId(ScenarioContext context, string orderId, OrganisationPartyPayload payload)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (payload == null)
                return;

            var payloadDictionary =
                context.Get<IDictionary<string, OrganisationPartyPayload>>(ScenarioContextKeys.OrganisationPayloadDictionary, new Dictionary<string, OrganisationPartyPayload>());

            payloadDictionary[orderId] = payload;

            if (!context.ContainsKey(ScenarioContextKeys.OrganisationPayloadDictionary))
            {
                context.Add(ScenarioContextKeys.OrganisationPayloadDictionary, payloadDictionary);
            }
        }

        [Given(@"an order party update request exist for order ID (.*)")]
        public void GivenAnOrderPartyUpdateRequestExistForOrderID(string orderId)
        {
            SetOrganisationPartyPayloadByOrderId(_context, orderId, new OrganisationPartyPayload());
        }

        [Given(@"the update request for order ID (.*) has a contact")]
        public void GivenTheUpdateRequestForOrderIdHasAContact(string orderId, Table table)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(_context, orderId);
            payload.PrimaryContact = table.CreateInstance<ContactPayload>();
        }

        [Given(@"the order party update request for order ID (.*) has a address")]
        public void GivenTheOrderPartyUpdateRequestForOrderIdHasAAddress(string orderId, Table table)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(_context, orderId);

            payload.Address = table.CreateInstance<AddressPayload>();
        }

        [Given(@"the order party update request for order ID (.*) has a Name of (.*)")]
        public void GivenTheOrderPartyUpdateRequestForOrderIdHasANameOfTestCareCenter(string orderId, string name)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(_context, orderId);

            payload.Name = name;
        }

        [Given(@"the order party update request for order ID (.*) has a OdsCode of (.*)")]
        public void GivenTheOrderPartyUpdateRequestForOrderIdHasAOrganisationOdsCodeOfTestCareOds(string orderId, string odsCode)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(_context, orderId);

            payload.OdsCode = odsCode;
        }

        [When(@"the user makes a request to retrieve the ordering-party section with the ID (.*)")]
        public async Task GivenTheUserMakesARequestToRetrieveTheOrdering_PartySectionWithTheID(string orderId)
        {
            await _request.GetAsync(string.Format(CultureInfo.InvariantCulture, _orderingPartyUrl, orderId));
        }

        [When(@"the user makes a request to update the order party on the order with the ID (.*)")]
        public async Task WhenTheUserMakesARequestToUpdateTheOrderPartyWithOrderId(string orderId)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(_context, orderId);
            await _request.PutJsonAsync(string.Format(CultureInfo.InvariantCulture, _orderingPartyUrl, orderId), payload);
        }

        [When(@"the user makes a request to update the order party with order ID (.*) with no model")]
        public async Task WhenTheUserMakesARequestToUpdateTheOrderPartyWithOrderIdWithNoModel(string orderId)
        {
            await _request.PutJsonAsync(string.Format(CultureInfo.InvariantCulture, _orderingPartyUrl, orderId), null);
        }

        [Then(@"the ordering-party is returned")]
        public async Task ThenTheOrdering_PartyOrganisationIsReturned(Table table)
        {
            var expected = table.CreateSet<OrganisationTable>().FirstOrDefault();

            var response = (await _response.ReadBodyAsJsonAsync());

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
            public string Name { get; set; }
            public string OdsCode { get; set; }
            public AddressPayload Address { get; set; }
            public ContactPayload PrimaryContact { get; set; }
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
