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
        private readonly Request _request;
        private readonly Settings _settings;
        private readonly OrderContext _orderContext;

        private UpdateOrderItemRequest _updateOrderItemRequest;

        public UpdateOrderItemSteps(
            Request request,
            Settings settings,
            OrderContext orderContext)
        {
            _request = request;
            _settings = settings;
            _orderContext = orderContext;
        }

        [Given(@"the user creates a request to change the order item \('(.*)'\) for the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToChangeTheOrderItemForTheOrderWithId(string name, string orderId)
        {
            var orderItemId = _orderContext.OrderItemReferenceList.GetByOrderCatalogueItemName(name).OrderItemId;

            _updateOrderItemRequest = new UpdateOrderItemRequest(
                _request, 
                _settings.OrderingApiBaseUrl,
                orderId,
                orderItemId);
        }

        [Given(@"the user enters the '(.*)' update order item request payload")]
        public void GivenTheUserEntersUpdateOrderItemRequestPayload(string payloadKey)
        {
            _updateOrderItemRequest.SetPayload(payloadKey);
        }

        [When(@"the user sends the update order item request")]
        public async Task WhenTheUserSendsTheUpdateOrderItemRequest()
        {
            await _updateOrderItemRequest.ExecuteAsync();
        }
        
        [Then(@"the order item is updated")]
        public async Task ThenTheOrderItemIsUpdated()
        {
            var original = _orderContext.OrderItemReferenceList
                .FindByOrderItemId(_updateOrderItemRequest.OrderItemId);

            var orderItem = await OrderItemEntity.FetchByOrderItemId(_settings.ConnectionString,
                _updateOrderItemRequest.OrderItemId);

            _updateOrderItemRequest.AssertPayload(orderItem, original);
        }
    }
}
