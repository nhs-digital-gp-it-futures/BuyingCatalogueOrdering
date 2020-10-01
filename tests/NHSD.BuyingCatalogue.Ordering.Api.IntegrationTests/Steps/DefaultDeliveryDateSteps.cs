using System;
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
    internal sealed class DefaultDeliveryDateSteps
    {
        private readonly Request request;
        private readonly Response response;
        private readonly Settings settings;

        private DefaultDeliveryDateEntity payload;

        public DefaultDeliveryDateSteps(Request request, Response response, Settings settings)
        {
            this.request = request;
            this.response = response;
            this.settings = settings;
        }

        [Given(@"the following default delivery dates have already been set")]
        public async Task GivenTheFollowingDefaultDeliveryDatesHaveAlreadyBeenSet(Table table)
        {
            var dates = table.CreateSet<DefaultDeliveryDateEntity>();
            foreach (var date in dates)
            {
                await date.InsertAsync(settings.ConnectionString);
            }
        }

        [Given(@"the user sets the default delivery date using the following details")]
        public void GivenTheUserSetsTheDefaultDeliveryDateUsingTheFollowingDetails(Table table)
        {
            SavePayload(table);
        }

        [When(@"the user confirms the default delivery date")]
        public async Task WhenTheUserConfirmsTheDefaultDeliveryDate()
        {
            await request.PutJsonAsync(
                settings.OrderingApiBaseUrl,
                new { payload.DeliveryDate },
                "api",
                "v1",
                "orders",
                payload.OrderId,
                "default-delivery-date",
                payload.CatalogueItemId,
                payload.PriceId);
        }

        [When(@"the user gets the default delivery date for the catalogue item with the following details")]
        public async Task WhenTheUserGetsTheDefaultDeliveryDateForTheCatalogueItemWithTheFollowingDetails(Table table)
        {
            SavePayload(table);

            await request.GetAsync(
                settings.OrderingApiBaseUrl,
                "api",
                "v1",
                "orders",
                payload.OrderId,
                "default-delivery-date",
                payload.CatalogueItemId,
                payload.PriceId);
        }

        [Then(@"the default delivery date is set correctly")]
        public async Task ThenTheDefaultDeliveryDateIsSetCorrectly()
        {
            await DateIsAsExpected(payload);
        }

        [Then(@"the default delivery date returned is (.*)")]
        public async Task ThenTheDefaultDeliveryDateReturnedIs(DateTime expectedDeliveryDate)
        {
            var result = await response.ReadBodyAsJsonAsync();
            var actualDate = result.Value<DateTime>("deliveryDate");

            actualDate.Should().Be(expectedDeliveryDate);
        }

        [Then(@"the following default delivery dates remain unchanged")]
        public async Task ThenTheFollowingDefaultDeliveryDatesRemainUnchanged(Table table)
        {
            var expectedDates = table.CreateSet<DefaultDeliveryDateEntity>();
            foreach (var expectedDate in expectedDates)
                await DateIsAsExpected(expectedDate);
        }

        private async Task DateIsAsExpected(DefaultDeliveryDateEntity expected)
        {
            var deliveryDate = await DefaultDeliveryDateEntity.Fetch(settings.ConnectionString, expected);
            deliveryDate.Should().NotBeNull();
            deliveryDate.Should().BeEquivalentTo(expected);
        }

        private void SavePayload(Table table) => payload = table.CreateInstance<DefaultDeliveryDateEntity>();
    }
}
