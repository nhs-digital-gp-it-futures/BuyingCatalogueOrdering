using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class HealthChecksSteps
    {
        private readonly Response response;
        private readonly ScenarioContext context;

        public HealthChecksSteps(Response response, ScenarioContext context, Settings settings)
        {
            this.response = response;
            this.context = context;
            this.context["orderingBaseUrl"] = settings.OrderingApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit for API")]
        public async Task WhenTheHealthCheckEndpointIsHit()
        {
            var baseUrl = context["orderingBaseUrl"];
            using var client = new HttpClient();
            response.Result = await client.GetAsync($"{baseUrl}/health/ready");
        }

        [Then(@"the response will be (Healthy|Degraded|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }
}
