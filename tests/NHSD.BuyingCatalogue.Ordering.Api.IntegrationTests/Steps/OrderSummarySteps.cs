using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderSummarySteps
    {
        private readonly Response _response;
        private readonly Request _request;

        private readonly string _orderSummaryUrl;

        public OrderSummarySteps(Response response, Request request, Settings settings)
        {
            _response = response;
            _request = request;
            _orderSummaryUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/summary";
        }

        [When(@"the user makes a request to retrieve the order summary with the ID (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheOrderSummaryWithTheId(string orderId)
        {
            await _request.GetAsync(string.Format(_orderSummaryUrl, orderId));
        }

        [Then(@"the order summary is returned with the following values")]
        public async Task ThenTheOrderSummaryIsReturnedWithTheFollowingValues(Table table)
        {
            var expected = table.CreateSet<OrderSummaryTable>().FirstOrDefault();

            var response = await _response.ReadBodyAsJsonAsync();

            var actual = new OrderSummaryTable
            {
                OrderId = response.Value<string>("orderId"),
                OrganisationId = response.SelectToken("organisationId").ToObject<Guid>(),
                Description = response.Value<string>("description")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the order Summary Sections have the following values")]
        public async Task ThenTheOrderSummarySectionsHaveTheFollowingValues(Table table)
        {
            var expected = table.CreateSet<SectionTable>();

            var response = await _response.ReadBodyAsJsonAsync();

            var actual = new SectionsTable
            {
                Sections = response.SelectToken("sections").ToObject<IEnumerable<SectionTable>>()
            };

            actual.Sections.Should().BeEquivalentTo(expected);
        }
        
        private sealed class OrderSummaryTable
        {
            public string OrderId { get; set; }

            public Guid OrganisationId { get; set; }

            public string Description { get; set; }
        }

        private sealed class SectionsTable
        {
            public IEnumerable<SectionTable> Sections { get; set; }
        }

        private sealed class SectionTable
        {
            public string Id { get; set; }
            public string Status { get; set; }
        }
    }
}
