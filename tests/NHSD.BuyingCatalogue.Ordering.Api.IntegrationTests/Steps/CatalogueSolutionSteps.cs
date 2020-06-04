using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal class CatalogueSolutionSteps
    {
        private readonly Request _request;
        private readonly Response _response;

        private readonly string _orderCatalogueSolutionsUrl;

        public CatalogueSolutionSteps(Request request, Response response, Settings settings)
        {
            _request = request;
            _response = response;
            _orderCatalogueSolutionsUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/catalogue-solutions";
        }

        [When(@"the user makes a request to retrieve the order catalogue solutions section with the ID (.*)")]
        public async Task WhenAGetRequestIsMadeForAnOrdersCatalogueSolutionsWithOrderId(string orderId)
        {
            await _request.GetAsync(string.Format(_orderCatalogueSolutionsUrl, orderId));
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
    }
}
