using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class UpdateOrderItemSteps
    {
        private readonly Request request;
        private readonly Settings settings;
        private readonly OrderContext orderContext;

        private UpdateOrderItemRequest updateOrderItemRequest;

        public UpdateOrderItemSteps(
            Request request,
            Settings settings,
            OrderContext orderContext)
        {
            this.request = request;
            this.settings = settings;
            this.orderContext = orderContext;
        }

        [Given(@"the user creates a request to change the order item \('(.*)'\) for the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToChangeTheOrderItemForTheOrderWithId(string name, string orderId)
        {
            var orderItemId = orderContext.OrderItemReferenceList.GetByOrderCatalogueItemName(name).OrderItemId;

            updateOrderItemRequest = new UpdateOrderItemRequest(
                request,
                settings.OrderingApiBaseUrl,
                orderId,
                orderItemId);
        }

        [Given(@"the user enters the '(.*)' update order item request payload")]
        public void GivenTheUserEntersUpdateOrderItemRequestPayload(string payloadKey)
        {
            updateOrderItemRequest.SetPayload(payloadKey);
        }

        [When(@"the user sends the update order item request")]
        public async Task WhenTheUserSendsTheUpdateOrderItemRequest()
        {
            await updateOrderItemRequest.ExecuteAsync();
        }

        [Then(@"the order item is updated")]
        public async Task ThenTheOrderItemIsUpdated()
        {
            var original = orderContext.OrderItemReferenceList
                .FindByOrderItemId(updateOrderItemRequest.OrderItemId);

            var orderItem = await OrderItemEntity.FetchByOrderItemId(
                settings.ConnectionString,
                updateOrderItemRequest.OrderItemId);

            updateOrderItemRequest.AssertPayload(orderItem, original);
        }
    }
}
