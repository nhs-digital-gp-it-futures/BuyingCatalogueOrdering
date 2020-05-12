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
        private readonly Response _response;
        private readonly ScenarioContext _context;

        public HealthChecksSteps(Response response, ScenarioContext context, Settings settings)
        {
            _response = response;
            _context = context;
            _context["orderingBaseUrl"] = settings.OrderingApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit for API")]
        public async Task WhenTheHealthCheckEndpointIsHit()
        {
            var baseUrl =_context["orderingBaseUrl"];
            using var client = new HttpClient();
            _response.Result = await client.GetAsync($"{baseUrl}/health/ready");
        }

        [Then(@"the response will be (Healthy|Degraded|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await _response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }

}
