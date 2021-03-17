using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class GetFundingSourceRequestSteps
    {
        private readonly Request request;
        private readonly Settings settings;
        private readonly OrderContext orderContext;
        private GetFundingSourceRequest getFundingSourceRequest;
        private GetFundingSourceResponse getFundingSourceResponse;

        public GetFundingSourceRequestSteps(
            Request request,
            Settings settings,
            OrderContext orderContext)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.orderContext = orderContext ?? throw new ArgumentNullException(nameof(orderContext));
        }

        [Given(@"the user creates a request to retrieve the funding source for order with ID (\d{1,6})")]
        public void GivenTheUserCreatesARequestToRetrieveTheFundingSourceForOrderWithId(int orderId)
        {
            getFundingSourceRequest = new GetFundingSourceRequest(
                request,
                settings.OrderingApiBaseUrl,
                orderId);
        }

        [When(@"the user sends the retrieve funding source request")]
        public async Task WhenTheUserSendsTheRetrieveFundingSourceRequest()
        {
            getFundingSourceResponse = await getFundingSourceRequest.ExecuteAsync();
        }

        [Then(@"the response contains the expected funding source details")]
        public void ThenTheResponseContainsTheExpectedFundingSourceDetails()
        {
            var order = orderContext.OrderReferenceList[getFundingSourceRequest.OrderId];
            getFundingSourceResponse.AssertBody(order.FundingSourceOnlyGms);
        }
    }
}
