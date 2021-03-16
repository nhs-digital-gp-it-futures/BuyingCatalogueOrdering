using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class UpdateOrderStatusRequest
    {
        private readonly Request request;
        private readonly string updateOrderStatusUrl;

        private readonly Dictionary<string, Func<UpdateOrderStatusRequestPayload>> payloadFactory = new()
        {
            { "order-status-complete", () => new UpdateOrderStatusRequestPayload { Status = "Complete" } },
            { "order-status-incomplete", () => new UpdateOrderStatusRequestPayload { Status = "Incomplete" } },
            { "order-status-invalid", () => new UpdateOrderStatusRequestPayload { Status = "INVALID" } },
            { "order-status-missing", () => new UpdateOrderStatusRequestPayload { Status = null } },
        };

        public UpdateOrderStatusRequest(Request request, string orderingApiBaseAddress, int orderId)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId;

            updateOrderStatusUrl = $"{orderingApiBaseAddress}/api/v1/orders/C{orderId}-01/status";
        }

        public int OrderId { get; set; }

        public UpdateOrderStatusRequestPayload Payload { get; private set; }

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
            await request.PutJsonAsync(updateOrderStatusUrl, Payload);
        }
    }
}
