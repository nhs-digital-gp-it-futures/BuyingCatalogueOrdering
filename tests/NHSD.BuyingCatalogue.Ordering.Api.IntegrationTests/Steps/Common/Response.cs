using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common
{
    internal sealed class Response
    {
        public HttpResponseMessage Result { get; set; }
    }
}
