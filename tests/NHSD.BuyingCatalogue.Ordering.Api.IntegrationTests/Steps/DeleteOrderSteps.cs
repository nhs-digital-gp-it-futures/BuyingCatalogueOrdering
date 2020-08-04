using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class DeleteOrderRequestSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;
        private DeleteOrderRequest _deleteOrderRequest;
        private Response _response;

        public DeleteOrderRequestSteps(
            Request request,
            Response response,
            Settings settings)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _response = response ?? throw new ArgumentNullException(nameof(response));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Given(@"the user creates a request to delete the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToDeleteAnOrderById(string orderId)
        {
            _deleteOrderRequest = new DeleteOrderRequest(
                _request,
                _settings.OrderingApiBaseUrl,
                orderId);
        }

        [When(@"the user sends the delete order request")]
        public async Task WhenTheUserSendsTheDeleteOrderRequest()
        {
            _response = await _deleteOrderRequest.ExecuteAsync();
        }

        [Then(@"the expected order is deleted")]
        public async Task ThenTheExpectedOrderIsDeleted()
        {
            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _deleteOrderRequest.OrderId);
            order.IsDeleted.Should().BeTrue();
        }
    }
}
