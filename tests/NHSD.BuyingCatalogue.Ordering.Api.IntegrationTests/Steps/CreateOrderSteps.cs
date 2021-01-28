using System;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class CreateOrderSteps
    {
        private readonly Response response;
        private readonly Request request;
        private readonly string orderingUrl;

        public CreateOrderSteps(
            Response response,
            Request request,
            Settings settings)
        {
            this.response = response;
            this.request = request;
            orderingUrl = settings.OrderingApiBaseUrl + "/api/v1/orders";
        }

        [When(@"a POST request is made to create an order")]
        public async Task WhenAOrderIsCreated(Table table)
        {
            var data = table.CreateInstance<CreateOrderPayload>();
            await request.PostJsonAsync(orderingUrl, data);
        }

        [Then(@"a create order response is returned with the OrderId (.*)")]
        public async Task ThenTheOrdersListIsReturnedWithTheFollowingValues(string orderId)
        {
            var responseOrderId = (await response.ReadBodyAsJsonAsync()).Value<string>("orderId");
            orderId.Should().Be(responseOrderId);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class CreateOrderPayload
        {
            public Guid OrganisationId { get; init; }

            public string Description { get; init; }
        }
    }
}
