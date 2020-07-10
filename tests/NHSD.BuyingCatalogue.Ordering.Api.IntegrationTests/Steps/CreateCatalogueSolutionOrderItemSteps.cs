using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class CreateCatalogueSolutionOrderItemSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;
        private CreateCatalogueSolutionOrderItemRequest _createCatalogueSolutionOrderItemRequest;
        private CreateCatalogueSolutionOrderItemResponse _createCatalogueSolutionOrderItemResponse;

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

        [Given(@"the user enters the '(.*)' create catalogue solution order item request payload")]
        public void GivenTheUserEntersPayload(string payloadTypeKey)
        {
            _createCatalogueSolutionOrderItemRequest.SetPayload(payloadTypeKey);
        }

        [When(@"the user sends the create catalogue solution order item request")]
        public async Task WhenTheUserSendsTheRequest()
        {
            _createCatalogueSolutionOrderItemResponse = await _createCatalogueSolutionOrderItemRequest.ExecuteAsync();
        }

        [Then(@"the expected catalogue solution order item is created")]
        public async Task ThenTheExpectedCatalogueSolutionOrderItemIsCreated()
        {
            var orderItem = await OrderItemEntity.FetchByCatalogueItemName(_settings.ConnectionString, _createCatalogueSolutionOrderItemRequest.Payload.CatalogueSolutionName);
            _createCatalogueSolutionOrderItemRequest.AssertPayload(orderItem);
        }

        [When(@"the response contains the new order item ID")]
        public async Task WhenTheResponseContainsTheNewOrderItemId()
        {
            var orderItem = await OrderItemEntity.FetchByCatalogueItemName(_settings.ConnectionString, _createCatalogueSolutionOrderItemRequest.Payload.CatalogueSolutionName);
            (await _createCatalogueSolutionOrderItemResponse.GetOrderItemIdAsync()).Should().Be(orderItem.OrderItemId);
        }

        [Then(@"the catalogue solution order section is marked as complete")]
        public async Task WhenTheCatalogueSolutionOrderSectionIsMarkedAsComplete()
        {
            var orderEntity = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _createCatalogueSolutionOrderItemRequest.OrderId);
            orderEntity.CatalogueSolutionsViewed.Should().BeTrue();
        }
    }
}
