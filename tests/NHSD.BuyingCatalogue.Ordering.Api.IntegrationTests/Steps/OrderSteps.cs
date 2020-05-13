using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderSteps
    {
        private readonly Response _response;
        private readonly Request _request;
        private readonly Settings _settings;

        private readonly string _orderOrganisationsUrl;

        public OrderSteps(Response response, Request request, Settings settings)
        {
            _response = response;
            _request = request;
            _settings = settings;
            _orderOrganisationsUrl = _settings.OrderingApiBaseUrl + "/api/v1/organisations/{0}/orders";
        }

        [Given(@"Orders exist")]
        public async Task GivenOrdersExit(Table table)
        {
            foreach (var ordersTableItem in table.CreateSet<OrdersTable>())
            {
                var order = OrderEntityBuilder
                    .Create()
                    .WithOrderId(ordersTableItem.OrderId)
                    .WithOrganisationId(ordersTableItem.OrganisationId)
                    .WithDescription(ordersTableItem.Description)
                    .WithOrderStatusId(ordersTableItem.OrderStatusId)
                    .WithDateCreated(ordersTableItem.Created)
                    .WithLastUpdatedBy(ordersTableItem.LastUpdatedBy)
                    .WithLastUpdated(ordersTableItem.LastUpdated)
                    .Build();

                await order.InsertAsync(_settings.ConnectionString);
            }
        }

        [When(@"a GET request is made for a list of orders with organisationId (.*)")]
        public async Task WhenAGETRequestIsMadeForOrders(Guid organisationId)
        {
            await _request.GetAsync(string.Format(_orderOrganisationsUrl, organisationId));
        }

        [Then(@"the orders list is returned with the following values")]
        public async Task ThenTheOrdersListIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrders = table.CreateSet<OrdersTable>();

            var orders = (await _response.ReadBodyAsJsonAsync()).Select(CreateOrders);

            orders.Should().BeEquivalentTo(expectedOrders,
                orderTable => orderTable.Excluding(order => order.OrderStatusId)
                    .Excluding(order => order.OrganisationId));
        }

        [Then(@"an empty list is returned")]
        public async Task AnEmptyListIsReturned()
        {
            var orders = (await _response.ReadBodyAsJsonAsync()).Select(CreateOrders);
            orders.Count().Should().Be(0);
        }

        private static object CreateOrders(JToken token)
        {
            return new
            {
                OrderId = token.Value<string>("orderId"),
                Description = token.Value<string>("description"),
                Status = token.Value<string>("status"),
                LastUpdated = token.Value<DateTime>("lastUpdated"),
                LastUpdatedBy = token.SelectToken("lastUpdatedBy").ToObject<Guid>(),
                Created = token.Value<DateTime>("dateCreated")
            };
        }

        private sealed class OrdersTable
        {
            public string OrderId { get; set; }

            public Guid OrganisationId { get; set; }

            public string Description { get; set; }

            public int OrderStatusId { get; set; }

            public string Status { get; set; }

            public DateTime Created { get; set; }

            public Guid LastUpdatedBy { get; set; }

            public DateTime LastUpdated { get; set; }
        }
    }
}
