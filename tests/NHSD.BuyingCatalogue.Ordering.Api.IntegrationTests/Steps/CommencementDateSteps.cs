using System;
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
        private readonly Request request;
        private readonly Response response;
        private readonly Settings settings;

        private readonly string orderCommencementDateUrl;
        private readonly ScenarioContext context;

        public CommencementDateSteps(
            Request request,
            Response response,
            Settings settings,
            ScenarioContext context)
        {
            this.request = request;
            this.response = response;
            this.settings = settings;
            this.context = context;

            orderCommencementDateUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/commencement-date";
        }

        [When(@"the user makes a request to retrieve the order commencement date section with the ID (.*)")]
        public async Task WhenAGetRequestIsMadeForAnOrdersCommencementDateWithOrderId(string orderId)
        {
            await request.GetAsync(string.Format(orderCommencementDateUrl, orderId));
        }

        [Then(@"the order commencement date is returned")]
        public async Task ThenTheOrderCommencementDateIsReturned(Table table)
        {
            var expected = table.CreateSet<CommencementDateTable>().FirstOrDefault();

            var apiReponse = await response.ReadBodyAsJsonAsync();

            var actual = new CommencementDateTable
            {
                CommencementDate = apiReponse.Value<DateTime?>("commencementDate"),
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Given(@"the user sets the commencement date to today")]
        public void GivenTheUserSetsCommencementDateToToday()
        {
            context["CommencementDate"] = DateTime.UtcNow;
        }

        [Given(@"the user sets the commencement date to nothing")]
        public void GivenTheUserSetsCommencementDateToNothing()
        {
            context["CommencementDate"] = null;
        }

        [Given(@"the user sets the commencement date to ([0-9]+) days in the past")]
        public void GivenTheUserSetsCommencementDateToThePast(int days)
        {
            context["CommencementDate"] = DateTime.UtcNow.Date - TimeSpan.FromDays(days);
        }

        [Given(@"the user sets the commencement date to ([0-9]+) days in the future")]
        public void GivenTheUserSetsCommencementDateToTheFuture(int days)
        {
            context["CommencementDate"] = DateTime.UtcNow + TimeSpan.FromDays(days);
        }

        [When(@"the user makes a request to update the commencement date with the ID (.*)")]
        public async Task WhenTheUserMakesARequestToUpdateTheCommencementDateWithOrderId(string orderId)
        {
            context.ContainsKey("CommencementDate").Should()
                .BeTrue("Commencement Date should have been set via the 'Given the user sets the commencement date' steps");
            var date = context["CommencementDate"] as DateTime?;
            await request.PutJsonAsync(
                string.Format(orderCommencementDateUrl, orderId),
                new { commencementDate = date });
        }

        [Then(@"the order commencement date for order with id (.*) is set to today")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsSetToToday(string orderId)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            actual.CommencementDate.HasValue.Should().BeTrue();
            actual.CommencementDate.Value.Date.Should().Be(DateTime.Today.Date);
        }

        [Then(@"the order commencement date for order with id (.*) is set to ([0-9]+) days in the future")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsSetToDaysInTheFuture(string orderId, int days)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            actual.CommencementDate.HasValue.Should().BeTrue();
            actual.CommencementDate.Value.Date.Should().Be(DateTime.Today.Date + TimeSpan.FromDays(days));
        }

        [Then(@"the order commencement date for order with id (.*) is set to ([0-9]+) days in the past")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsSetToDaysAgo(string orderId, int days)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            actual.CommencementDate.HasValue.Should().BeTrue();
            actual.CommencementDate.Value.Date.Should().Be(DateTime.Today.Date - TimeSpan.FromDays(days));
        }

        private sealed class CommencementDateTable
        {
            public DateTime? CommencementDate { get; set; }
        }
    }
}
