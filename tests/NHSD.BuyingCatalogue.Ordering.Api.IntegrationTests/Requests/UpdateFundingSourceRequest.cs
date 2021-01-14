using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal class UpdateFundingSourceRequest
    {
        private readonly Request request;
        private readonly string updateFundingSourceUrl;

        private readonly Dictionary<string, Func<UpdateFundingSourceRequestPayload>> payloadFactory = new()
        {
            { "funding-source-true", () => new UpdateFundingSourceRequestPayload { OnlyGms = true } },
            { "funding-source-false", () => new UpdateFundingSourceRequestPayload { OnlyGms = false } },
            { "funding-source-missing", () => new UpdateFundingSourceRequestPayload { OnlyGms = null } },
        };

        public UpdateFundingSourceRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            updateFundingSourceUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/funding-source";
        }

        public string OrderId { get; set; }

        public UpdateFundingSourceRequestPayload Payload { get; private set; }

        public void SetPayload(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (!payloadFactory.TryGetValue(key, out var factory))
                Assert.Fail("Unexpected update funding source payload type.");

            Payload = factory();
        }

        public async Task ExecuteAsync()
        {
            await request.PutJsonAsync(updateFundingSourceUrl, Payload);
        }
    }
}
