using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
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
        private readonly Response response;
        private readonly Settings settings;

        private BulkRequest bulkRequest;

        public CreateOrderItemBulkSteps(
            Request request,
            Response response,
            Settings settings)
        {
            this.request = request;
            this.response = response;
            this.settings = settings;
        }

        [Given(@"the user creates a request to add the following items to the order with ID '(.*)'")]
        public async Task GivenTheUserCreatesARequestToAddTheFollowingItemsToTheOrderWithId(string orderId, Table requestInfoTable)
        {
            await CreateBulkRequest(
                requestInfoTable,
                orderId,
                new CreateOrderItemRequestForCreateFactory(request, settings, orderId));
        }

        [Given(@"the user creates a request to update the order with ID '(.*)' with the following items")]
        public async Task GivenTheUserCreatesARequestToUpdateTheFollowingItemsToTheOrderWithId(string orderId, Table requestInfoTable)
        {
            await CreateBulkRequest(
                requestInfoTable,
                orderId,
                new CreateOrderItemRequestForUpdateFactory(request, settings, orderId));
        }

        [When(@"the user sends the create order items request")]
        [When(@"the user sends the edit order items request")]
        public async Task WhenTheUserSendsTheCreateOrderItemsRequest()
        {
            await bulkRequest.SendRequest(request);
        }

        [Then(@"the expected order items are created")]
        [Then(@"the expected order items exist")]
        public async Task ThenTheExpectedOrderItemsAreCreated()
        {
            await bulkRequest.OrderItemsCreatedAsExpected();
        }

        [Then(@"the response contains the following error details")]
        public async Task ThenTheResponseContainsTheFollowingErrorDetails(Table table)
        {
            IReadOnlyList<dynamic> expectedErrorDetails = table.CreateDynamicSet().ToList();

            var expectedErrors = new Dictionary<string, string[]>();
            for (var i = 0; i < expectedErrorDetails.Count; i++)
            {
                var expectedErrorDetail = expectedErrorDetails[i];
                if (string.IsNullOrWhiteSpace(expectedErrorDetail.Field))
                    continue;

                expectedErrors.Add($"[{i}].{expectedErrorDetail.Field}", new string[] { expectedErrorDetail.Id });
            }

            var responseBody = await response.ReadBodyAsStringAsync();
            var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseBody);

            problemDetails.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        private async Task CreateBulkRequest(
            Table requestInfoTable,
            string orderId,
            CreateOrderItemRequestFactory itemFactory)
        {
            var requestInfo = requestInfoTable.CreateSet<RequestInfo>();
            bulkRequest = new BulkRequest(orderId, settings.OrderingApiBaseUrl, settings.ConnectionString);

            foreach (var info in requestInfo)
            {
                bulkRequest.Items.Add(await itemFactory.CreateOrderItemRequest(info));
            }
        }

        private abstract class CreateOrderItemRequestFactory
        {
            private readonly string orderId;
            private readonly Request request;
            private readonly Settings settings;

            protected CreateOrderItemRequestFactory(
                Request request,
                Settings settings,
                string orderId)
            {
                this.orderId = orderId;
                this.request = request;
                this.settings = settings;
            }

            internal CreateOrderItemBaseRequest OrderItemRequest { get; private set; }

            internal async Task<CreateOrderItemBaseRequest> CreateOrderItemRequest(RequestInfo requestInfo)
            {
                OrderItemRequest = CreateOrderItemBaseRequest.Create(requestInfo.ItemType, request, settings.OrderingApiBaseUrl, orderId);
                OrderItemRequest.SetPayload(requestInfo.PayloadType);

                return await InitializeOrderItemRequest(requestInfo);
            }

            protected abstract Task<CreateOrderItemBaseRequest> InitializeOrderItemRequest(RequestInfo requestInfo);

            protected async Task<CreateOrderItemBaseRequest> SetAdditionalPayloadValues(RequestInfo requestInfo)
            {
                var payload = OrderItemRequest.Payload;

                if (!string.IsNullOrWhiteSpace(requestInfo.CatalogueItemName))
                    payload.CatalogueItemName = requestInfo.CatalogueItemName;

                if (!string.IsNullOrWhiteSpace(requestInfo.OdsCode))
                    payload.OdsCode = requestInfo.OdsCode;

                if (!string.IsNullOrWhiteSpace(requestInfo.ServiceRecipientName))
                    payload.ServiceRecipientName = requestInfo.ServiceRecipientName;

                payload.OrderItemId = requestInfo.OrderItemId;

                if (payload.HasOrderItemId)
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    // Use the admin connection to prevent exception on service failure test
                    OrderItemRequest.OriginalEntity = await OrderItemEntity.FetchByOrderItemId(
                        settings.OrderingDbAdminConnectionString,
                        payload.OrderItemId.Value);
                }

                return OrderItemRequest;
            }
        }

        private sealed class CreateOrderItemRequestForCreateFactory : CreateOrderItemRequestFactory
        {
            internal CreateOrderItemRequestForCreateFactory(
                Request request,
                Settings settings,
                string orderId)
                : base(request, settings, orderId)
            {
            }

            protected override async Task<CreateOrderItemBaseRequest> InitializeOrderItemRequest(RequestInfo requestInfo)
            {
                if (string.IsNullOrWhiteSpace(requestInfo.ServiceRecipientName))
                    return OrderItemRequest;

                return await SetAdditionalPayloadValues(requestInfo);
            }
        }

        private sealed class CreateOrderItemRequestForUpdateFactory : CreateOrderItemRequestFactory
        {
            internal CreateOrderItemRequestForUpdateFactory(
                Request request,
                Settings settings,
                string orderId)
                : base(request, settings, orderId)
            {
            }

            protected override async Task<CreateOrderItemBaseRequest> InitializeOrderItemRequest(RequestInfo requestInfo) =>
                await SetAdditionalPayloadValues(requestInfo);
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

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class RequestInfo
        {
            public int? OrderItemId { get; set; }

            public string ItemType { get; set; }

            public string PayloadType { get; set; }

            public string CatalogueItemName { get; set; }

            public string OdsCode { get; set; }

            public string ServiceRecipientName { get; set; }
        }
    }
}
