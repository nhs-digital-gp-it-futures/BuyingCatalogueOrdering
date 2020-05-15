﻿using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderDescriptionSteps
    {
        private readonly Response _response;
        private readonly Request _request;
        private readonly Settings _settings;

        private readonly string _orderDescriptionUrl;

        public OrderDescriptionSteps(Response response, Request request, Settings settings)
        {
            _response = response;
            _request = request;
            _settings = settings;
            _orderDescriptionUrl = _settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/description";
        }

        [When(@"the user makes a request to retrieve the order description section with the ID (.*)")]
        public async Task WhenAgetRequestIsMadeForAnOrdersDescriptionWithOrderId(string orderId)
        {
            await _request.GetAsync(string.Format(_orderDescriptionUrl, orderId));
        }

        [Then(@"the order description is returned")]
        public async Task ThenTheOrderDescriptionIsReturned(Table table)
        {
            var expected = table.CreateSet<OrderDescriptionTable>().FirstOrDefault();

            var response = await _response.ReadBodyAsJsonAsync();

            var actual = new OrderDescriptionTable()
            {
                Description = response.SelectToken("description").ToString()
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [When(@"the user makes a request to update the description with the ID (.*)")]
        public async Task WhenTheUserMakesARequestToUpdateTheDescriptionWithOrderId(string orderId, Table table)
        {
            var data = table.CreateInstance<OrderDescriptionTable>();

            await _request.PutJsonAsync(string.Format(_orderDescriptionUrl, orderId), data);
        }


        private sealed class OrderDescriptionTable
        {
            public string Description { get; set; }
        }
    }
}
