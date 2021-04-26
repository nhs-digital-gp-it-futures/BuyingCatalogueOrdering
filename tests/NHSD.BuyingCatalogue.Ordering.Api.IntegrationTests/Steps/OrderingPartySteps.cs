using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderingPartySteps
    {
        private readonly Response response;
        private readonly Request request;
        private readonly string orderingPartyUrl;
        private readonly OrderContext orderContext;
        private readonly ScenarioContext context;
        private readonly Settings settings;

        public OrderingPartySteps(
            Response response,
            Request request,
            Settings settings,
            ScenarioContext context,
            OrderContext orderContext)
        {
            this.response = response;
            this.request = request;
            this.context = context;
            this.orderContext = orderContext;
            this.settings = settings;
            orderingPartyUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/C{0}-01/sections/ordering-party";
        }

        [Given(@"ordering parties exist")]
        public async Task GivenOrderingPartiesExist(Table table)
        {
            foreach (var entity in table.CreateSet<OrderingPartyEntity>())
            {
                await entity.InsertAsync(settings.ConnectionString);
                orderContext.OrderingPartyReferenceList.Add(entity.Id, entity);
            }
        }

        [Given(@"an order party update request exist for order ID (\d{1,6})")]
        public void GivenAnOrderPartyUpdateRequestExistForOrderId(int orderId)
        {
            SetOrganisationPartyPayloadByOrderId(context, orderId, new OrganisationPartyPayload());
        }

        [Given(@"the update request for order ID (\d{1,6}) has a contact")]
        public void GivenTheUpdateRequestForOrderIdHasAContact(int orderId, Table table)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(context, orderId);
            payload.PrimaryContact = table.CreateInstance<ContactPayload>();
        }

        [Given(@"the order party update request for order ID (\d{1,6}) has an address")]
        public void GivenTheOrderPartyUpdateRequestForOrderIdHasAAddress(int orderId, Table table)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(context, orderId);

            payload.Address = table.CreateInstance<AddressPayload>();
        }

        [Given(@"the order party update request for order ID (\d{1,6}) has a Name of (.*)")]
        public void GivenTheOrderPartyUpdateRequestForOrderIdHasANameOf(int orderId, string name)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(context, orderId);

            payload.Name = name;
        }

        [Given(@"the order party update request for order ID (\d{1,6}) has a OdsCode of (.*)")]
        public void GivenTheOrderPartyUpdateRequestForOrderIdHasAOrganisationOdsCodeOfTestCareOds(
            int orderId,
            string odsCode)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(context, orderId);

            payload.OdsCode = odsCode;
        }

        [When(@"the user makes a request to retrieve the ordering-party section for the order with ID (\d{1,6})")]
        public async Task GivenTheUserMakesARequestToRetrieveTheOrdering_PartySectionWithTheID(int orderId)
        {
            await request.GetAsync(string.Format(CultureInfo.InvariantCulture, orderingPartyUrl, orderId));
        }

        [When(@"the user makes a request to update the order party on the order with the ID (\d{1,6})")]
        public async Task WhenTheUserMakesARequestToUpdateTheOrderPartyWithOrderId(int orderId)
        {
            var payload = GetOrganisationPartyPayloadByOrderId(context, orderId);
            await request.PutJsonAsync(string.Format(CultureInfo.InvariantCulture, orderingPartyUrl, orderId), payload);
        }

        [When(@"the user makes a request to update the order party with order ID (\d{1,6}) with no model")]
        public async Task WhenTheUserMakesARequestToUpdateTheOrderPartyWithOrderIdWithNoModel(int orderId)
        {
            await request.PutJsonAsync(string.Format(CultureInfo.InvariantCulture, orderingPartyUrl, orderId), null);
        }

        [Then(@"the ordering-party is returned")]
        public async Task ThenTheOrdering_PartyOrganisationIsReturned(Table table)
        {
            var expected = table.CreateSet<OrderingPartyTable>().FirstOrDefault();

            var jsonResponse = await response.ReadBodyAsJsonAsync();

            var actual = new OrderingPartyTable
            {
                Name = jsonResponse.Value<string>("name"),
                OdsCode = jsonResponse.Value<string>("odsCode"),
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the ordering party with ID (.*) has the following details")]
        public async Task ThenTheOrderingPartyWithIdHasTheFollowingDetails(Guid id, Table table)
        {
            var actual = await OrderingPartyEntity.FetchById(settings.ConnectionString, id);
            table.CompareToInstance(actual);
        }

        private static OrganisationPartyPayload GetOrganisationPartyPayloadByOrderId(
            ScenarioContext context,
            int orderId)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var payloadDictionary = context.Get<IDictionary<int, OrganisationPartyPayload>>(
                ScenarioContextKeys.OrganisationPayloadDictionary,
                new Dictionary<int, OrganisationPartyPayload>());

            return payloadDictionary.TryGetValue(orderId, out var payload) ? payload : null;
        }

        private static void SetOrganisationPartyPayloadByOrderId(
            ScenarioContext context,
            int orderId,
            OrganisationPartyPayload payload)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (payload is null)
                return;

            var payloadDictionary = context.Get<IDictionary<int, OrganisationPartyPayload>>(
                ScenarioContextKeys.OrganisationPayloadDictionary,
                new Dictionary<int, OrganisationPartyPayload>());

            payloadDictionary[orderId] = payload;

            if (!context.ContainsKey(ScenarioContextKeys.OrganisationPayloadDictionary))
            {
                context.Add(ScenarioContextKeys.OrganisationPayloadDictionary, payloadDictionary);
            }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OrderingPartyTable
        {
            public Guid Id { get; init; }

            public string Name { get; init; }

            public string OdsCode { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OrganisationPartyPayload
        {
            public string Name { get; set; }

            public string OdsCode { get; set; }

            public AddressPayload Address { get; set; }

            public ContactPayload PrimaryContact { get; set; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class AddressPayload
        {
            public string Line1 { get; init; }

            public string Line2 { get; init; }

            public string Line3 { get; init; }

            public string Line4 { get; init; }

            public string Line5 { get; init; }

            public string Town { get; init; }

            public string County { get; init; }

            public string Postcode { get; init; }

            public string Country { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ContactPayload
        {
            public string FirstName { get; init; }

            public string LastName { get; init; }

            public string EmailAddress { get; init; }

            public string TelephoneNumber { get; init; }
        }
    }
}
