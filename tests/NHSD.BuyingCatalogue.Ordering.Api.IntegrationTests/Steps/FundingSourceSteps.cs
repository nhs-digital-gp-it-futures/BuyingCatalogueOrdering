using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class FundingSourceSteps
    {
        private readonly Request request;
        private readonly Settings settings;

        private UpdateFundingSourceRequest fundingSourceRequest;

        public FundingSourceSteps(Request request, Settings settings)
        {
            this.request = request;
            this.settings = settings;
        }

        [Given(@"the user creates a request to update the funding source for the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToUpdateFundingSourceForOrderWithId(string orderId)
        {
            fundingSourceRequest = new UpdateFundingSourceRequest(request, settings.OrderingApiBaseUrl, orderId);
        }

        [Given(@"the user enters the '(.*)' update funding source request payload")]
        public void GivenTheUserEntersPayload(string payloadTypeKey)
        {
            fundingSourceRequest.SetPayload(payloadTypeKey);
        }

        [When(@"the user sends the update funding source request")]
        public async Task WhenTheUserSendsTheRequest()
        {
            await fundingSourceRequest.ExecuteAsync();
        }

        [Then(@"the funding source is set correctly")]
        public async Task ThenTheExpectedCatalogueSolutionOrderItemIsCreated()
        {
            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, fundingSourceRequest.OrderId);
            order.FundingSourceOnlyGms.Should().Be(fundingSourceRequest.Payload.OnlyGms);
        }
    }
}
