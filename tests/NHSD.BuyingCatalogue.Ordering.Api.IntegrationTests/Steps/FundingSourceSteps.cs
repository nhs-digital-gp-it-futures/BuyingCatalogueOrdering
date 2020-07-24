using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    class FundingSourceSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;

        private UpdateFundingSourceRequest _fundingSourceRequest;

        public FundingSourceSteps(Request request, Settings settings)
        {
            _request = request;
            _settings = settings;
        }

        [Given(@"the user creates a request to update the funding source for the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToUpdateFundingSourceForOrderWithId(string orderId)
        {
            _fundingSourceRequest = new UpdateFundingSourceRequest(_request, _settings.OrderingApiBaseUrl, orderId);
        }

        [Given(@"the user enters the '(.*)' update funding source request payload")]
        public void GivenTheUserEntersPayload(string payloadTypeKey)
        {
            _fundingSourceRequest.SetPayload(payloadTypeKey);
        }

        [When(@"the user sends the update funding source request")]
        public async Task WhenTheUserSendsTheRequest()
        {
            await _fundingSourceRequest.ExecuteAsync();
        }

        [Then(@"the funding source is set correctly")]
        public async Task ThenTheExpectedCatalogueSolutionOrderItemIsCreated()
        {
            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _fundingSourceRequest.OrderId);
            order.FundingSourceOnlyGMS.Should().Be(_fundingSourceRequest.Payload.OnlyGMS);
        }
    }
}
