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
        private readonly Response _response;
        private readonly ScenarioContext _context;

        public Request(Response response, ScenarioContext context)
        {
            _response = response;
            _context = context;
        }

        public async Task<Response> GetAsync(string url, params object[] pathSegments)
        {
            _response.Result = await CreateCommonRequest(url, pathSegments).GetAsync();
            return _response;
        }

        public async Task<Response> DeleteAsync(string url, params object[] pathSegments)
        {
            _response.Result = await CreateCommonRequest(url, pathSegments).DeleteAsync();
            return _response;
        }

        public async Task<Response> PostJsonAsync(string url, object payload, params object[] pathSegments)
        {
            _response.Result = await CreateCommonRequest(url, pathSegments).PostJsonAsync(payload);
            return _response;
        }

        public async Task PutJsonAsync(string url, object payload, params object[] pathSegments)
            => _response.Result = await CreateCommonRequest(url, pathSegments).PutJsonAsync(payload);

        private IFlurlRequest CreateCommonRequest(string url, params object[] pathSegments)
        {
            string accessToken = null;

            if (_context.ContainsKey(ScenarioContextKeys.AccessToken))
            {
                accessToken = _context.Get<string>(ScenarioContextKeys.AccessToken);
            }

            return url
                .AppendPathSegments(pathSegments)
                .WithOAuthBearerToken(accessToken)
                .AllowAnyHttpStatus();
        }
    }
}
