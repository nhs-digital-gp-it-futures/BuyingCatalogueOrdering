using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class CreateOrderItemSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;
        private CreateOrderItemBaseRequest _createOrderItemRequest;
        private CreateOrderItemResponse _createOrderItemResponse;

        public CreateOrderItemSteps(
            Request request,
            Settings settings)
        {
            _request = request;
            _settings = settings;
        }

        [Given(@"the user creates a request to add a new (.*) order item to the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToAddANewCatalogueSolutionOrderItemToTheOrderWithId(string itemType, string orderId)
        {
            _createOrderItemRequest = CreateOrderItemBaseRequest.Create(itemType, _request, _settings.OrderingApiBaseUrl, orderId);
        }

        [Given(@"the user enters the '(.*)' create order item request payload")]
        public void GivenTheUserEntersPayload(string payloadTypeKey)
        {
            _createOrderItemRequest.SetPayload(payloadTypeKey);
        }

        [When(@"the user sends the create order item request")]
        public async Task WhenTheUserSendsTheRequest()
        {
            _createOrderItemResponse = await _createOrderItemRequest.ExecuteAsync();
        }

        [Then(@"the expected order item is created")]
        public async Task ThenTheExpectedCatalogueSolutionOrderItemIsCreated()
        {
            var orderItem = await OrderItemEntity.FetchByCatalogueItemName(_settings.ConnectionString, _createOrderItemRequest.Payload.CatalogueItemName);
            _createOrderItemRequest.AssertPayload(orderItem);
        }

        [When(@"the response contains the new order item ID")]
        public async Task WhenTheResponseContainsTheNewOrderItemId()
        {
            var orderItem = await OrderItemEntity.FetchByCatalogueItemName(_settings.ConnectionString, _createOrderItemRequest.Payload.CatalogueItemName);
            (await _createOrderItemResponse.GetOrderItemIdAsync()).Should().Be(orderItem.OrderItemId);
        }

        [Then(@"the catalogue solution order section is marked as complete")]
        public async Task WhenTheCatalogueSolutionOrderSectionIsMarkedAsComplete()
        {
            var orderEntity = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _createOrderItemRequest.OrderId);
            orderEntity.CatalogueSolutionsViewed.Should().BeTrue();
        }

        [Then(@"the additional service order section is marked as complete")]
        public async Task WhenTheAdditionalServiceOrderSectionIsMarkedAsComplete()
        {
            var orderEntity = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _createOrderItemRequest.OrderId);
            orderEntity.AdditionalServicesViewed.Should().BeTrue();
        }

        [Then(@"the associated service order section is marked as complete")]
        public async Task WhenTheAssociatedServiceOrderSectionIsMarkedAsComplete()
        {
            var orderEntity = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _createOrderItemRequest.OrderId);
            orderEntity.AssociatedServicesViewed.Should().BeTrue();
        }

        [Then(@"the order item estimation period is set to '(.*)'")]
        public async Task ThenTheCatalogueSolutionOrderItemEstimationPeriodIsSetTo(string estimationPeriodName)
        {
            TimeUnit? expectedEstimationPeriod = null;
            if (!string.IsNullOrWhiteSpace(estimationPeriodName))
                expectedEstimationPeriod = Enum.Parse<TimeUnit>(estimationPeriodName, true);

            var orderItem = await OrderItemEntity.FetchByCatalogueItemName(_settings.ConnectionString, _createOrderItemRequest.Payload.CatalogueItemName);
            orderItem.EstimationPeriod.Should().Be(expectedEstimationPeriod);
        }
    }
}
