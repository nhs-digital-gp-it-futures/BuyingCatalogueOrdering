using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class CreateCatalogueSolutionOrderItemSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;
        private CreateCatalogueSolutionOrderItemRequest _createCatalogueSolutionOrderItemRequest;

        public CreateCatalogueSolutionOrderItemSteps(
            Request request,
            Settings settings)
        {
            _request = request;
            _settings = settings;
        }

        [Given(@"the user creates a request to add a new catalogue solution order item to the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToAddANewCatalogueSolutionOrderItemToTheOrderWithId(string orderId)
        {
            _createCatalogueSolutionOrderItemRequest = new CreateCatalogueSolutionOrderItemRequest(
                _request,
                _settings.OrderingApiBaseUrl,
                orderId);
        }

        [Given(@"the user enters '(.*)' payload")]
        public void GivenTheUserEntersPayload(string payloadTypeKey)
        {
            if (!PayloadFactory.TryGetValue(payloadTypeKey, out var factory))
                Assert.Fail("Unexpected create catalogue solution order item payload type.");

            _createCatalogueSolutionOrderItemRequest.Payload = factory();
        }

        [When(@"the user sends the request")]
        public async Task WhenTheUserSendsTheRequest()
        {
            await _createCatalogueSolutionOrderItemRequest.ExecuteAsync();
        }

        [Then(@"the expected catalogue solution order item is created")]
        public async Task ThenTheExpectedCatalogueSolutionOrderItemIsCreated()
        {
            var orderItem = await OrderItemEntity.FetchByCatalogueItemName(_settings.ConnectionString, _createCatalogueSolutionOrderItemRequest.Payload.CatalogueSolutionName);
            _createCatalogueSolutionOrderItemRequest.AssertPayload(orderItem);
        }

        private static readonly IDictionary<string, Func<CreateCatalogueSolutionOrderItemRequestPayload>> PayloadFactory =
            new Dictionary<string, Func<CreateCatalogueSolutionOrderItemRequestPayload>>()
            {
                {
                    "complete", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().Build()
                }
            };
    }
}
