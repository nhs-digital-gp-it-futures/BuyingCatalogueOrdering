using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderDescriptionSteps
    {
        private readonly Response response;
        private readonly Request request;
        private readonly Settings settings;

        private readonly string orderDescriptionUrl;

        public OrderDescriptionSteps(Response response, Request request, Settings settings)
        {
            this.response = response;
            this.request = request;
            this.settings = settings;

            orderDescriptionUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/description";
        }

        [When(@"the user makes a request to retrieve the order description section with the ID (.*)")]
        public async Task WhenAGetRequestIsMadeForAnOrdersDescriptionWithOrderId(string orderId)
        {
            await request.GetAsync(string.Format(CultureInfo.InvariantCulture, orderDescriptionUrl, orderId));
        }

        [Then(@"the order description is returned")]
        public async Task ThenTheOrderDescriptionIsReturned(Table table)
        {
            var expected = table.CreateSet<OrderDescriptionTable>().FirstOrDefault();

            var jsonResponse = await response.ReadBodyAsJsonAsync();

            var actual = new OrderDescriptionTable
            {
                Description = jsonResponse.Value<string>("description"),
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [When(@"the user makes a request to update the description with the ID (.*)")]
        public async Task WhenTheUserMakesARequestToUpdateTheDescriptionWithOrderId(string orderId, Table table)
        {
            var data = table.CreateInstance<OrderDescriptionTable>();

            await request.PutJsonAsync(string.Format(CultureInfo.InvariantCulture, orderDescriptionUrl, orderId), data);
        }

        [When(@"the user makes a request to update the description with the ID (.*) with no model")]
        public async Task WhenTheUserMakesARequestToUpdateTheDescriptionWithOrderIdWithNoModel(string orderId)
        {
            await request.PutJsonAsync(string.Format(CultureInfo.InvariantCulture, orderDescriptionUrl, orderId), null);
        }

        [Then(@"the order description for order with id (.*) is set to")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsSetTo(string orderId, Table table)
        {
            var expected = table.CreateInstance<OrderDescriptionTable>().Description;

            var actual = (await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId)).Description;
            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the lastUpdatedName is updated in the database to (.*) with orderId (.*)")]
        public async Task ThenTheLastUpdatedNameIsUpdatedInTheDatabase(string expected, string orderId)
        {
            var actual = (await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId)).LastUpdatedByName;
            actual.Should().BeEquivalentTo(expected);
        }

        private sealed class OrderDescriptionTable
        {
            public string Description { get; init; }
        }
    }
}
