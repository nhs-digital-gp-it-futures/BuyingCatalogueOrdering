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
        private readonly Request request;
        private readonly Settings settings;
        private readonly OrderContext orderContext;
        private GetOrderItemRequest getOrderItemRequest;
        private GetOrderItemResponse getOrderItemResponse;

        public GetOrderItemSteps(
            Request request,
            Settings settings,
            OrderContext orderContext)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.orderContext = orderContext ?? throw new ArgumentNullException(nameof(orderContext));
        }

        [Given(@"the user creates a request to retrieve an order item with catalogue item ID '(.*)' and order ID (\d{1,6})")]
        public void GivenTheUserCreatesARequestToRetrieveAnOrderItemWithCatalogueItemNameAndOrderId(
            string catalogueItemId,
            int orderId)
        {
            getOrderItemRequest = new GetOrderItemRequest(
                request,
                settings.OrderingApiBaseUrl,
                orderId,
                catalogueItemId);
        }

        [Given(@"the user creates a request to retrieve an order item that does not exist")]
        public void GivenTheUserCreatesARequestToRetrieveAnOrderItemThatDoesNotExist()
        {
            var order = orderContext.OrderReferenceList.Values.First();

            getOrderItemRequest = new GetOrderItemRequest(
                request,
                settings.OrderingApiBaseUrl,
                order.Id,
                "999-09");
        }

        [Given(@"the user creates a request to retrieve an order item for an order that does not exist")]
        public void GivenTheUserCreatesARequestToRetrieveAnOrderItemForAnOrderThatDoesNotExist()
        {
            getOrderItemRequest = new GetOrderItemRequest(
                request,
                settings.OrderingApiBaseUrl,
                99,
                "999");
        }

        [When(@"the user sends the retrieve an order item request")]
        public async Task WhenTheUserSendsTheRetrieveAnOrderItemRequest()
        {
            getOrderItemResponse = await getOrderItemRequest.ExecuteAsync();
        }

        [Then(@"the response contains the expected order item")]
        public void ThenTheResponseContainsTheExpectedOrderItem()
        {
            var orderItem = orderContext.OrderItemReferenceList[getOrderItemRequest.OrderId][getOrderItemRequest.CatalogueItemId];
            var orderItemRecipients = orderContext.OrderItemRecipientsReferenceList[(orderItem.OrderId, orderItem.CatalogueItemId)];

            getOrderItemResponse.AssertBody(
                orderItem,
                orderContext.ServiceRecipientReferenceList,
                orderItemRecipients,
                orderContext.PricingUnitReferenceList);
        }
    }
}
