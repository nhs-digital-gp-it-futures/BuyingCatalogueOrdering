using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal class CatalogueSolutionSteps
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly Settings _settings;

        private readonly string _orderCatalogueSolutionsUrl;

        public CatalogueSolutionSteps(Request request, Response response, Settings settings)
        {
            _request = request;
            _response = response;
            _settings = settings;
            _orderCatalogueSolutionsUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/catalogue-solutions";
        }

        [When(@"the user makes a request to retrieve the order catalogue solutions section with the ID (.*)")]
        public async Task WhenAGetRequestIsMadeForAnOrdersCatalogueSolutionsWithOrderId(string orderId)
        {
            await _request.GetAsync(string.Format(_orderCatalogueSolutionsUrl, orderId));
        }

        [When(@"the user makes a request to update the order catalogue solutions section with the ID (.*)")]
        public async Task WhenAPutRequestIsMadeForAnOrdersCatalogueSolutionsWithOrderId(string orderId)
        {
            await _request.PutJsonAsync(string.Format(_orderCatalogueSolutionsUrl, orderId), null);
        }

        [Then(@"the catalogue solutions response contains the order description (.*)")]
        public async Task AnEmptyListIsReturned(string description)
        {
            var response = await _response.ReadBodyAsJsonAsync();
            var actual = response.Value<string>("orderDescription");
            actual.Should().Be(description);
        }

        [Then(@"the catalogue solutions response contains no solutions")]
        public async Task TheCatalogueSolutionsResponseContainsNoSolutions()
        {
            var response = await _response.ReadBodyAsJsonAsync();
            var solutionToken = response.SelectToken("catalogueSolutions");
            var solutions = solutionToken.Select(x => new { name = x.Value<string>("name") });
            solutions.Should().BeEmpty();
        }

        [Then(@"the order with ID (.*) has catalogue solutions viewed set to (true|false)")]
        public async Task TheOrderWithIdHasCatalogueSolutionsViewedSet(string orderId, bool viewed)
        {
            var actual = (await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId)).CatalogueSolutionsViewed;
            actual.Should().Be(viewed);
        }
    }
}
