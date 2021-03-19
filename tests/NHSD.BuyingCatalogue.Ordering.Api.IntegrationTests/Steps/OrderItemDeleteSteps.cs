using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
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
        private readonly Response response;
        private readonly Settings settings;
        private readonly OrderContext orderContext;
        private DeleteOrderItemRequest deleteOrderItemRequest;
        private int originalOrderItemsCount;

        public OrderItemDeleteSteps(
            Request request,
            Response response,
            Settings settings,
            OrderContext orderContext)
        {
            this.request = request;
            this.response = response;
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.orderContext = orderContext;
        }

        [Given(@"the user creates a request to delete order item with catalogue item ID (.*) for order ID (\d{1,6})")]
        public void GivenTheUserCreatesARequestToDeleteOrderItemForOrder(string catalogueItemId, int orderId)
        {
            originalOrderItemsCount = orderContext.OrderItemReferenceList.Count;

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

        [Then(@"the expected order items and additional services are deleted")]
        public async Task ThenTheExpectedOrderItemsAndAdditonalServicesAreDeleted()
        {
            var orderItemsCount = await OrderItemEntity.GetOrderItemCountForOrder(
                settings.ConnectionString,
                deleteOrderItemRequest.OrderId);

            orderItemsCount.Should().Be(1);
        }

        [Then(@"the order is not updated")]
        public async Task ThenTheOrderIsNotUpdated()
        {
            var finalOrderItemsCount = await OrderItemEntity.GetOrderItemCountForOrder(
                settings.ConnectionString,
                deleteOrderItemRequest.OrderId);

            finalOrderItemsCount.Should().Be(originalOrderItemsCount);
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

                CatalogueItemId = catalogueItemId;
                OrderId = orderId;
                deleteOrderUrl = $"{orderingApiBaseAddress}/api/v1/orders/C0{orderId}-01/order-items/{catalogueItemId}";
            }

            public string CatalogueItemId { get; }

            public int OrderId { get; }

            public async Task ExecuteAsync() => await request.DeleteAsync(deleteOrderUrl);
        }
    }
}
