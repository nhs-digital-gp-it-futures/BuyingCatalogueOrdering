using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common
{
    [Binding]
    internal sealed class CommonSteps
    {
        private readonly Response response;

        public CommonSteps(Response response)
        {
            this.response = response;
        }

        [Then(@"a response with status code ([\d]+) is returned")]
        public void AResponseIsReturned(int code)
        {
            response.Should().NotBeNull();
            response.Result.StatusCode.Should().Be(code);
        }

        [Then(@"the response contains the following errors")]
        public async Task ThenTheResponseContainsTheFollowingErrors(Table table)
        {
            var expected = table.CreateSet<ResponseErrorsTable>();

            var jsonResponse = await response.ReadBodyAsJsonAsync();

            var actual = jsonResponse?.SelectToken("errors")?.Select(t => new ResponseErrorsTable
            {
                Id = t.Value<string>("id"),
                Field = t.Value<string>("field"),
            });

            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the response contains no data")]
        public async Task TheResponseContainsNoData()
        {
            var jsonResponse = await response.ReadBodyAsStringAsync();
            jsonResponse.Should().Be("{}");
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ResponseErrorsTable
        {
            public string Id { get; init; }

            public string Field { get; init; }
        }
    }
}
