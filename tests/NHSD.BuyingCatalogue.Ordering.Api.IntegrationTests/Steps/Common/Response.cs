using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common
{
    public sealed class Response
    {
        public HttpResponseMessage Result { get; set; }

        public async Task<JToken> ReadBodyAsJsonAsync() => JToken.Parse(await Result.Content.ReadAsStringAsync());
    }
}
