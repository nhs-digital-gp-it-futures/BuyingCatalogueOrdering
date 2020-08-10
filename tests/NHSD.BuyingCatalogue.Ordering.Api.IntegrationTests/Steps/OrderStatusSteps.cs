using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal class OrderStatusSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;

        private UpdateOrderStatusRequest _updateOrderStatusRequest;

        public OrderStatusSteps(Request request, Settings settings)
        {
            _request = request;
            _settings = settings;
        }

        [Given(@"the user creates a request to update the order status for the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToUpdateTheStatusForTheOrderWithId(string orderId)
        {
            _updateOrderStatusRequest = new UpdateOrderStatusRequest(_request, _settings.OrderingApiBaseUrl, orderId);
        }

        [Given(@"the user enters the '(.*)' update order status request payload")]
        public void GivenTheUserEntersTheUpdateOrderStatusRequestPayload(string payloadTypeKey)
        {
            _updateOrderStatusRequest.SetPayload(payloadTypeKey);
        }

        [When(@"the user sends the update order status request")]
        public async Task WhenTheUserSendsTheUpdateOrderStatusRequest()
        {
            await _updateOrderStatusRequest.ExecuteAsync();
        }

        [Then(@"the order status is set correctly")]
        public async Task ThenTheOrderStatusIsSetCorrectly()
        {
            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _updateOrderStatusRequest.OrderId);
            Enum.TryParse<OrderStatus>(_updateOrderStatusRequest.Payload.Status, out var orderStatus);

            order.OrderStatus.Should().Be(orderStatus);
        }

        [Then(@"the order completed date is set")]
        public async Task ThenTheOrderCompletedDateIsSet()
        {
            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _updateOrderStatusRequest.OrderId);
            order.Completed.Should().NotBeNull();
        }
    }
}
