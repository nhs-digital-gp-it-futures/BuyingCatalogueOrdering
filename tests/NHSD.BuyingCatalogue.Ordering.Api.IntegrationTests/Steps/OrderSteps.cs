using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
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
                    .WithLastUpdatedName(ordersTableItem.LastUpdatedByName)
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
                    .Excluding(order => order.OrganisationId)
                    .Excluding(order => order.LastUpdatedBy));
        }

        [Then(@"an empty list is returned")]
        public async Task AnEmptyListIsReturned()
        {
            var orders = (await _response.ReadBodyAsJsonAsync()).Select(CreateOrders);
            orders.Count().Should().Be(0);
        }

        [Then(@"the order is created in the database with orderId (.*) and data")]
        public async Task ThenTheOrderIsCreatedInTheDatabase(string orderId, Table table)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            table.CompareToInstance<OrderEntity>(actual);
        }

        [Then(@"the order with orderId (.*) has LastUpdated time present and it is the current time")]
        public async Task ThenOrderOrderIdHasLastUpdatedAtCurrentTime(string orderId)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            actual.LastUpdated.Should().BeWithin(TimeSpan.FromSeconds(3)).Before(DateTime.UtcNow);
        }

        [Then(@"the order with orderId (.*) has Created time present and it is the current time")]
        public async Task ThenOrderOrderIdHasCreatedAtCurrentTime(string orderId)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            actual.Created.Should().BeWithin(TimeSpan.FromSeconds(3)).Before(DateTime.UtcNow);
        }

        private static object CreateOrders(JToken token)
        {
            return new
            {
                OrderId = token.Value<string>("orderId"),
                Description = token.Value<string>("description"),
                Status = token.Value<string>("status"),
                LastUpdated = token.Value<DateTime>("lastUpdated"),
                LastUpdatedByName = token.Value<string>("lastUpdatedBy"),
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

            public string LastUpdatedByName { get; set; }

            public DateTime LastUpdated { get; set; }
        }
    }
}
