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
        private readonly Request _request;
        private readonly Settings _settings;
        private readonly OrderContext _orderContext;
        private GetFundingSourceRequest _getFundingSourceRequest;
        private GetFundingSourceResponse _getFundingSourceResponse;

        public GetFundingSourceRequestSteps(
            Request request,
            Settings settings,
            OrderContext orderContext)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _orderContext = orderContext ?? throw new ArgumentNullException(nameof(orderContext));
        }

        [Given(@"the user creates a request to retrieve the funding source for order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToRetrieveTheFundingSourceForOrderWithId(string orderId)
        {
            _getFundingSourceRequest = new GetFundingSourceRequest(
                _request, 
                _settings.OrderingApiBaseUrl, 
                orderId);
        }
        
        [When(@"the user sends the retrieve funding source request")]
        public async Task WhenTheUserSendsTheRetrieveFundingSourceRequest()
        {
            _getFundingSourceResponse = await _getFundingSourceRequest.ExecuteAsync();
        }
        
        [Then(@"the response contains the expected funding source details")]
        public void ThenTheResponseContainsTheExpectedFundingSourceDetails()
        {
            var order = _orderContext.OrderReferenceList.GetByOrderId(_getFundingSourceRequest.OrderId);
            _getFundingSourceResponse.AssertBody(order.FundingSourceOnlyGMS);
        }
    }
}
