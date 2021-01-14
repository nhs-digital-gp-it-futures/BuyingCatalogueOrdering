using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class DeleteOrderRequestSteps
    {
        private readonly Request request;
        private readonly Settings settings;
        private DeleteOrderRequest deleteOrderRequest;

        public DeleteOrderRequestSteps(
            Request request,
            Settings settings)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Given(@"the user creates a request to delete the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToDeleteAnOrderById(string orderId)
        {
            deleteOrderRequest = new DeleteOrderRequest(
                request,
                settings.OrderingApiBaseUrl,
                orderId);
        }

        [When(@"the user sends the delete order request")]
        public async Task WhenTheUserSendsTheDeleteOrderRequest()
        {
            await deleteOrderRequest.ExecuteAsync();
        }

        [Then(@"the expected order is deleted")]
        public async Task ThenTheExpectedOrderIsDeleted()
        {
            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, deleteOrderRequest.OrderId);
            order.IsDeleted.Should().BeTrue();
        }
    }
}
