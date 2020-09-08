﻿using System;
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
    internal sealed class CommencementDateSteps
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly Settings _settings;

        private readonly string _orderCommencementDateUrl;
        private readonly ScenarioContext _context;

        public CommencementDateSteps(Request request, Response response, Settings settings, ScenarioContext context)
        {
            _request = request;
            _response = response;
            _settings = settings;
            _context = context;

            _orderCommencementDateUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/commencement-date";
        }

        [When(@"the user makes a request to retrieve the order commencement date section with the ID (.*)")]
        public async Task WhenAGetRequestIsMadeForAnOrdersCommencementDateWithOrderId(string orderId)
        {
            await _request.GetAsync(string.Format(_orderCommencementDateUrl, orderId));
        }

        [Then(@"the order commencement date is returned")]
        public async Task ThenTheOrderCommencementDateIsReturned(Table table)
        {
            var expected = table.CreateSet<CommencementDateTable>().FirstOrDefault();

            var response = await _response.ReadBodyAsJsonAsync();

            var actual = new CommencementDateTable
            {
                CommencementDate = response.Value<DateTime?>("commencementDate")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Given(@"the user sets the commencement date to today")]
        public void GivenTheUserSetsCommencementDateToToday()
        {
            _context["CommencementDate"] = DateTime.Now;
        }

        [Given(@"the user sets the commencement date to nothing")]
        public void GivenTheUserSetsCommencementDateToNothing()
        {
            _context["CommencementDate"] = null;
        }

        [Given(@"the user sets the commencement date to ([0-9]+) days in the past")]
        public void GivenTheUserSetsCommencementDateToThePast(int days)
        {
            _context["CommencementDate"] = DateTime.Now - TimeSpan.FromDays(days);
        }

        [Given(@"the user sets the commencement date to ([0-9]+) days in the future")]
        public void GivenTheUserSetsCommencementDateToTheFuture(int days)
        {
            _context["CommencementDate"] = DateTime.Now + TimeSpan.FromDays(days);
        }

        [When(@"the user makes a request to update the commencement date with the ID (.*)")]
        public async Task WhenTheUserMakesARequestToUpdateTheCommencementDateWithOrderId(string orderId)
        {
            _context.ContainsKey("CommencementDate").Should()
                .BeTrue("Commencement Date should have been set via the 'Given the user sets the commencement date' steps");
            var date = _context["CommencementDate"] as DateTime?;
            await _request.PutJsonAsync(string.Format(_orderCommencementDateUrl, orderId),
                new {commencementDate = date});
        }

        [Then(@"the order commencement date for order with id (.*) is set to today")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsSetToToday(string orderId)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            actual.CommencementDate.HasValue.Should().BeTrue();
            actual.CommencementDate.Value.Date.Should().Be(DateTime.Today.Date);
        }

        [Then(@"the order commencement date for order with id (.*) is set to ([0-9]+) days in the future")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsSetToDaysInTheFuture(string orderId, int days)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            actual.CommencementDate.HasValue.Should().BeTrue();
            actual.CommencementDate.Value.Date.Should().Be(DateTime.Today.Date + TimeSpan.FromDays(days));
        }

        [Then(@"the order commencement date for order with id (.*) is set to ([0-9]+) days in the past")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsSetToDaysAgo(string orderId, int days)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            actual.CommencementDate.HasValue.Should().BeTrue();
            actual.CommencementDate.Value.Date.Should().Be(DateTime.Today.Date - TimeSpan.FromDays(days));
        }

        private sealed class CommencementDateTable
        {
            public DateTime? CommencementDate { get; set; }
        }
    }
}
