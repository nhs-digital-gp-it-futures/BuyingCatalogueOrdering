using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class SectionStatusUpdateSteps
    {
        private readonly Response _response;
        private readonly Request _request;
        private readonly Settings _settings;
        private readonly string _sectionStatusUpdateUrl;

        public SectionStatusUpdateSteps(Response response, Request request, Settings settings)
        {
            _response = response;
            _request = request;
            _settings = settings;
            _sectionStatusUpdateUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/{1}";
        }

        [When(@"the user makes a request to complete order section with order Id (.*) section Id (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheOrderSummaryWithTheId(string orderId, string sectionId)
        {
            var payload = new {Status = "complete"};
            await _request.PutJsonAsync(string.Format(_sectionStatusUpdateUrl, orderId, sectionId), payload);
        }

        [Then(@"the order with ID (.*) has additional services viewed set to (.*)")]
        public async Task TheOrderWithIdHasCatalogueSolutionsViewedSet(string orderId, bool viewed)
        {
            var actual = (await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId)).AdditionalServicesViewed;
            actual.Should().Be(viewed);
        }

        [Then(@"the order with ID (.*) has associated services viewed set to (.*)")]
        public async Task TheOrderWithIdHasAssociatedServicesViewedSet(string orderId, bool viewed)
        {
            var actual = (await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId)).AssociatedServicesViewed;
            actual.Should().Be(viewed);
        }
    }
}
