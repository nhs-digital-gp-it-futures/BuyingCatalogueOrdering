using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class GetOrderItemSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;
        private readonly OrderContext _orderContext;
        private GetOrderItemRequest _getOrderItemRequest;
        private GetOrderItemResponse _getOrderItemResponse;

        public GetOrderItemSteps(
            Request request,
            Settings settings,
            OrderContext orderContext)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _orderContext = orderContext ?? throw new ArgumentNullException(nameof(orderContext));
        }

        [Given(@"the user creates a request to retrieve an order item with catalogue item name '(.*)' and order ID '(.*)'")]
        public void GivenTheUserCreatesARequestToRetrieveAnOrderItemWithCatalogueItemNameAndOrderId(
            string catalogueSolutionName,
            string orderId)
        {
            var orderItem = _orderContext.OrderItemReferenceList.GetByOrderCatalogueItemName(catalogueSolutionName);

            _getOrderItemRequest = new GetOrderItemRequest(
                _request,
                _settings.OrderingApiBaseUrl,
                orderId,
                orderItem.OrderItemId);
        }

        [Given(@"the user creates a request to retrieve an order item that does not exist")]
        public void GivenTheUserCreatesARequestToRetrieveAnOrderItemThatDoesNotExist()
        {
            var order = _orderContext.OrderReferenceList.GetAll().First();

            _getOrderItemRequest = new GetOrderItemRequest(
                _request,
                _settings.OrderingApiBaseUrl,
                order.OrderId,
                999);
        }

        [Given(@"the user creates a request to retrieve an order item for an order that does not exist")]
        public void GivenTheUserCreatesARequestToRetrieveAnOrderItemForAnOrderThatDoesNotExist()
        {
            var orderItem = _orderContext.OrderItemReferenceList.GetAll().First();

            _getOrderItemRequest = new GetOrderItemRequest(
                _request,
                _settings.OrderingApiBaseUrl,
                Guid.NewGuid().ToString(),
                orderItem.OrderItemId);
        }

        [When(@"the user sends the retrieve an order item request")]
        public async Task WhenTheUserSendsTheRetrieveAnOrderItemRequest()
        {
            _getOrderItemResponse = await _getOrderItemRequest.ExecuteAsync();
        }

        [Then(@"the response contains the expected order item")]
        public void ThenTheResponseContainsTheExpectedOrderItem()
        {
            var orderItem = _orderContext
                .OrderItemReferenceList.FindByOrderItemId(_getOrderItemRequest.OrderItemId);

            var serviceRecipient = _orderContext
                .ServiceRecipientReferenceList.Get(_getOrderItemRequest.OrderId, orderItem.OdsCode);

            _getOrderItemResponse.AssertBody(orderItem, serviceRecipient);
        }
    }
}
