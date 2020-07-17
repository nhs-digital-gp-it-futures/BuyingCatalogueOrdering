using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class UpdateCatalogueSolutionOrderItemSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;
        private readonly OrderContext _orderContext;

        private UpdateCatalogueSolutionOrderItemRequest _updateCatalogueSolutionOrderItemRequest;

        public UpdateCatalogueSolutionOrderItemSteps(
            Request request,
            Settings settings,
            OrderContext orderContext)
        {
            _request = request;
            _settings = settings;
            _orderContext = orderContext;
        }

        [Given(@"the user creates a request to change the catalogue solution order item \('(.*)'\) for the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToChangeTheCatalogueSolutionOrderItemForTheOrderWithId(string name, string orderId)
        {
            var orderItemId = _orderContext.OrderItemReferenceList.GetByOrderCatalogueItemName(name).OrderItemId;

            _updateCatalogueSolutionOrderItemRequest = new UpdateCatalogueSolutionOrderItemRequest(
                _request, 
                _settings.OrderingApiBaseUrl,
                orderId,
                orderItemId);
        }

        [Given(@"the user enters the '(.*)' update catalogue solution order item request payload")]
        public void GivenTheUserEntersUpdateCatalogueSolutionOrderItemRequestPayload(string payloadKey)
        {
            _updateCatalogueSolutionOrderItemRequest.SetPayload(payloadKey);
        }

        [When(@"the user sends the update catalogue solution order item request")]
        public async Task WhenTheUserSendsTheUpdateCatalogueSolutionOrderItemRequest()
        {
            await _updateCatalogueSolutionOrderItemRequest.ExecuteAsync();
        }
        
        [Then(@"the catalogue solution order item is updated")]
        public async Task ThenTheCatalogueSolutionOrderItemIsUpdated()
        {
            var orderItem = await OrderItemEntity.FetchByOrderItemId(_settings.ConnectionString,
                _updateCatalogueSolutionOrderItemRequest.OrderItemId);
            var original = _orderContext.OrderItemReferenceList
                    .FindByOrderItemId(_updateCatalogueSolutionOrderItemRequest
                    .OrderItemId);

            _updateCatalogueSolutionOrderItemRequest.AssertPayload(orderItem, original);
        }
    }
}
