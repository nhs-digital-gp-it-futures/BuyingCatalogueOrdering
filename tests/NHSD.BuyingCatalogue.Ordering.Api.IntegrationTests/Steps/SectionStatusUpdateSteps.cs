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
            sectionStatusUpdateUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/{1}";
        }

        [When(@"the user makes a request to complete order section with order Id (.*) section Id (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheOrderSummaryWithTheId(string orderId, string sectionId)
        {
            var payload = new { Status = "complete" };
            await request.PutJsonAsync(string.Format(CultureInfo.InvariantCulture, sectionStatusUpdateUrl, orderId, sectionId), payload);
        }

        [Then(@"the order with ID (.*) has catalogue solutions viewed set to (.*)")]
        public async Task TheOrderWithIdHasCatalogueSolutionsViewedSet(string orderId, bool viewed)
        {
            var actual = (await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId)).CatalogueSolutionsViewed;
            actual.Should().Be(viewed);
        }

        [Then(@"the order with ID (.*) has additional services viewed set to (.*)")]
        public async Task TheOrderWithIdHasAdditionalServicesViewedSet(string orderId, bool viewed)
        {
            var actual = (await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId)).AdditionalServicesViewed;
            actual.Should().Be(viewed);
        }

        [Then(@"the order with ID (.*) has associated services viewed set to (.*)")]
        public async Task TheOrderWithIdHasAssociatedServicesViewedSet(string orderId, bool viewed)
        {
            var actual = (await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId)).AssociatedServicesViewed;
            actual.Should().Be(viewed);
        }
    }
}
