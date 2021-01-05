using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    internal sealed class Request
    {
        private readonly Response response;
        private readonly ScenarioContext context;

        public Request(Response response, ScenarioContext context)
        {
            this.response = response;
            this.context = context;
        }

        public async Task<Response> GetAsync(string url, params object[] pathSegments)
        {
            response.Result = (await CreateCommonRequest(url, pathSegments).GetAsync()).ResponseMessage;
            return response;
        }

        public async Task<Response> DeleteAsync(string url, params object[] pathSegments)
        {
            response.Result = (await CreateCommonRequest(url, pathSegments).DeleteAsync()).ResponseMessage;
            return response;
        }

        public async Task<Response> PostJsonAsync(string url, object payload, params object[] pathSegments)
        {
            response.Result = (await CreateCommonRequest(url, pathSegments).PostJsonAsync(payload)).ResponseMessage;
            return response;
        }

        public async Task PutJsonAsync(string url, object payload, params object[] pathSegments)
            => response.Result = (await CreateCommonRequest(url, pathSegments).PutJsonAsync(payload)).ResponseMessage;

        private IFlurlRequest CreateCommonRequest(string url, params object[] pathSegments)
        {
            string accessToken = null;

            if (context.ContainsKey(ScenarioContextKeys.AccessToken))
            {
                accessToken = context.Get<string>(ScenarioContextKeys.AccessToken);
            }

            return url
                .AppendPathSegments(pathSegments)
                .WithOAuthBearerToken(accessToken)
                .AllowAnyHttpStatus();
        }
    }
}
