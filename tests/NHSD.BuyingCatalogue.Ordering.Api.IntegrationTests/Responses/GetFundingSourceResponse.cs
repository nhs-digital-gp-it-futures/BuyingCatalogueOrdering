using System;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetFundingSourceResponse
    {
        private readonly string content;

        private GetFundingSourceResponse(string content)
        {
            this.content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public static async Task<GetFundingSourceResponse> CreateAsync(Response response)
        {
            var content = await response.ReadBodyAsStringAsync();
            return new GetFundingSourceResponse(content);
        }

        public void AssertBody(bool? expectedFundingSourceOnlyGms)
        {
            var body = JToken.Parse(content);

            var actual = body.Value<bool?>("onlyGMS");

            actual.Should().Be(expectedFundingSourceOnlyGms);
        }
    }
}
