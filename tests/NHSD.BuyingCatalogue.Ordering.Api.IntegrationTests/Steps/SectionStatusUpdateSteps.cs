using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class SectionStatusUpdateSteps
    {
        private readonly Request request;
        private readonly Settings settings;
        private readonly string sectionStatusUpdateUrl;

        public SectionStatusUpdateSteps(Request request, Settings settings)
        {
            this.request = request;
            this.settings = settings;
            sectionStatusUpdateUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/C{0}-01/sections/{1}";
        }

        [When(@"the user makes a request to complete order section with order Id (\d{1,6}) section Id (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheOrderSummaryWithTheId(int orderId, string sectionId)
        {
            var payload = new { Status = "complete" };
            await request.PutJsonAsync(string.Format(CultureInfo.InvariantCulture, sectionStatusUpdateUrl, orderId, sectionId), payload);
        }

        [Then(@"the order with ID (\d{1,6}) has catalogue solutions viewed set to (.*)")]
        public async Task TheOrderWithIdHasCatalogueSolutionsViewedSet(int orderId, bool viewed)
        {
            await ProgressPropertyHasExpectedValue(orderId, p => p.CatalogueSolutionsViewed, viewed);
        }

        [Then(@"the order with ID (\d{1,6}) has additional services viewed set to (.*)")]
        public async Task TheOrderWithIdHasAdditionalServicesViewedSet(int orderId, bool viewed)
        {
            await ProgressPropertyHasExpectedValue(orderId, p => p.AdditionalServicesViewed, viewed);
        }

        [Then(@"the order with ID (\d{1,6}) has associated services viewed set to (.*)")]
        public async Task TheOrderWithIdHasAssociatedServicesViewedSet(int orderId, bool viewed)
        {
            await ProgressPropertyHasExpectedValue(orderId, p => p.AssociatedServicesViewed, viewed);
        }

        private async Task ProgressPropertyHasExpectedValue(
            int orderId,
            Func<OrderProgressEntity, bool> propertySelector,
            bool expectedValue)
        {
            var progress = await OrderProgressEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            var actual = propertySelector(progress);
            actual.Should().Be(expectedValue);
        }
    }
}
