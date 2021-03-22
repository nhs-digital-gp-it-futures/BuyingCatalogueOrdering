using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderItemDeleteSteps
    {
        private readonly Request request;
        private readonly Settings settings;
        private DeleteOrderItemRequest deleteOrderItemRequest;

        public OrderItemDeleteSteps(
            Request request,
            Settings settings)
        {
            this.request = request;
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Given(@"the user creates a request to delete order item with catalogue item ID (.*) for the order with ID (\d{1,6})")]
        public void GivenTheUserCreatesARequestToDeleteOrderItemForOrder(string catalogueItemId, int orderId)
        {
            deleteOrderItemRequest = new DeleteOrderItemRequest(
                request,
                settings.OrderingApiBaseUrl,
                catalogueItemId,
                orderId);
        }

        [When(@"the user sends the delete order item request")]
        public async Task WhenTheUserSendsTheDeleteOrderItemRequest()
        {
            await deleteOrderItemRequest.ExecuteAsync();
        }

        private sealed class DeleteOrderItemRequest
        {
            private readonly Request request;
            private readonly string deleteOrderUrl;

            public DeleteOrderItemRequest(
                Request request,
                string orderingApiBaseAddress,
                string catalogueItemId,
                int orderId)
            {
                this.request = request ?? throw new ArgumentNullException(nameof(request));

                deleteOrderUrl = $"{orderingApiBaseAddress}/api/v1/orders/C0{orderId}-01/order-items/{catalogueItemId}";
            }

            public async Task ExecuteAsync() => await request.DeleteAsync(deleteOrderUrl);
        }
    }
}
