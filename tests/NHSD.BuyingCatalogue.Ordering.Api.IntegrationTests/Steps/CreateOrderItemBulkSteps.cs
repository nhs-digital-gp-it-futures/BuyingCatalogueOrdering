using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class CreateOrderItemBulkSteps
    {
        private readonly Request request;
        private readonly Settings settings;

        private BulkRequest bulkRequest;

        public CreateOrderItemBulkSteps(
            Request request,
            Settings settings)
        {
            this.request = request;
            this.settings = settings;
        }

        [Given(@"the user creates a request to add the following items to the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToAddTheFollowingItemsToTheOrderWithId(string orderId, Table requestInfoTable)
        {
            var requestInfo = requestInfoTable.CreateSet<RequestInfo>();
            bulkRequest = new BulkRequest(orderId, settings.OrderingApiBaseUrl, settings.ConnectionString);

            foreach (var info in requestInfo)
            {
                bulkRequest.Items.Add(CreateItemRequest(info, orderId));
            }
        }

        [When(@"the user sends the create order items request")]
        public async Task WhenTheUserSendsTheCreateOrderItemsRequest()
        {
            await bulkRequest.SendRequest(request);
        }

        [Then(@"the expected order items are created")]
        public async Task ThenTheExpectedOrderItemsAreCreated()
        {
            await bulkRequest.OrderItemsCreatedAsExpected();
        }

        private CreateOrderItemBaseRequest CreateItemRequest(RequestInfo info, string orderId)
        {
            var itemRequest = CreateOrderItemBaseRequest.Create(info.ItemType, request, settings.OrderingApiBaseUrl, orderId);
            itemRequest.SetPayload(info.PayloadType);

            var payload = itemRequest.Payload;
            payload.CatalogueItemName = info.CatalogueItemName;
            payload.OdsCode = info.OdsCode;
            payload.ServiceRecipientName = info.ServiceRecipientName;

            return itemRequest;
        }

        private sealed class BulkRequest
        {
            private readonly string dbConnectionString;
            private readonly Uri url;

            internal BulkRequest(string orderId, string orderingApiBaseUrl, string dbConnectionString)
            {
                this.dbConnectionString = dbConnectionString;
                url = new Uri($"{orderingApiBaseUrl}/api/v1/orders/{orderId}/order-items/batch");
            }

            internal IList<CreateOrderItemBaseRequest> Items { get; } = new List<CreateOrderItemBaseRequest>();

            internal async Task SendRequest(Request request)
            {
                await request.PostJsonAsync(url.ToString(), Items.Select(p => p.CreatePayload()).ToArray());
            }

            internal async Task OrderItemsCreatedAsExpected()
            {
                foreach (var request in Items)
                {
                    var orderItem = await OrderItemEntity.FetchByCatalogueItemName(dbConnectionString, request.Payload.CatalogueItemName);
                    request.AssertPayload(orderItem);
                }
            }
        }

        private sealed class RequestInfo
        {
            public string ItemType { get; set; }

            public string PayloadType { get; set; }

            public string CatalogueItemName { get; set; }

            public string OdsCode { get; set; }

            public string ServiceRecipientName { get; set; }
        }
    }
}
