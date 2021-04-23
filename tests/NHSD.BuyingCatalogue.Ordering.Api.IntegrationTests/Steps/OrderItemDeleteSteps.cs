using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderItemDeleteSteps
    {
        private readonly Request request;
        private readonly Settings settings;
        private readonly OrderContext orderContext;
        private DeleteOrderItemRequest deleteOrderItemRequest;

        public OrderItemDeleteSteps(
            Request request,
            Settings settings,
            OrderContext orderContext)
        {
            this.request = request;
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.orderContext = orderContext ?? throw new ArgumentNullException(nameof(orderContext));
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

        [Then(@"the fundingSourceOnlyGms value is null")]
        public async Task ThenTheFundingSourceOnlyGmsIs()
        {
            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, deleteOrderItemRequest.OrderId);
            order.FundingSourceOnlyGms.Should().BeNull();
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
                OrderId = orderId;
                deleteOrderUrl = $"{orderingApiBaseAddress}/api/v1/orders/C0{orderId}-01/order-items/{catalogueItemId}";
            }

            internal int OrderId { get; }

            public async Task ExecuteAsync() => await request.DeleteAsync(deleteOrderUrl);
        }
    }
}
