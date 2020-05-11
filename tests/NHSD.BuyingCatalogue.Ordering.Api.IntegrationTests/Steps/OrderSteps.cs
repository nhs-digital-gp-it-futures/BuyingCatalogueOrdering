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

        private readonly string _orderUrl;

        public OrderSteps(Response response, Request request, Settings settings)
        {
            _response = response;
            _request = request;
            _settings = settings;
            _orderUrl = _settings.OrderingApiBaseUrl + "/api/v1/orders";
        }

        [Given(@"Orders Exit")]
        public async Task GivenOrdersExit(Table table)
        {
            foreach (var ordersTableItem in table.CreateSet<OrdersTable>())
            {
                var order = OrderEntityBuilder
                    .Create()
                    .WithOrderId(ordersTableItem.OrderId)
                    .WithDescription(ordersTableItem.Description)
                    .WithOrderStatusId(ordersTableItem.OrderStatusId)
                    .WithDateCreated(ordersTableItem.Created)
                    .WithLastUpdatedBy(ordersTableItem.LastUpdatedBy)
                    .WithLastUpdated(ordersTableItem.LastUpdated)
                    .Build();

                await order.InsertAsync(_settings.ConnectionString);
            }
        }

        [When(@"a GET request is made for orders")]
        public async Task WhenAGETRequestIsMadeForOrders()
        {
            await _request.GetAsync(_orderUrl);
        }

        [Then(@"the orders list is returned with the following values")]
        public async Task ThenTheOrdersListIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrders = table.CreateSet<OrdersTable>();

            var orders = (await _response.ReadBodyAsJsonAsync()).Select(CreateOrders);

            orders.Should().BeEquivalentTo(expectedOrders, a => a.Excluding(u => u.OrderStatusId));
        }

        private static object CreateOrders(JToken token)
        {
            return new
            {
                OrderId = token.SelectToken("orderId").ToString(),
                Description = token.SelectToken("orderDescription").ToString(),
                Status = token.SelectToken("status").ToString(),
                LastUpdated = token.SelectToken("lastUpdated").ToObject<DateTime>(),
                LastUpdatedBy = token.SelectToken("lastUpdatedBy").ToObject<Guid>(),
                Created = token.SelectToken("dateCreated").ToObject<DateTime>()
            };
        }

        private class OrdersTable
        {
            public string OrderId { get; set; }

            public string Description { get; set; }

            public int OrderStatusId { get; set; }

            public string Status { get; set; }

            public DateTime Created { get; set; }

            public Guid LastUpdatedBy { get; set; }

            public DateTime LastUpdated { get; set; }
        }
    }
}
