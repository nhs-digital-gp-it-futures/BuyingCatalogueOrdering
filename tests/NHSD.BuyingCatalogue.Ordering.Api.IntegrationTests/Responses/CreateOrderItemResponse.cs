using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class CreateOrderItemResponse
    {
        private readonly Response response;

        public CreateOrderItemResponse(
            Response response)
        {
            this.response = response;
        }

        public async Task<int?> GetOrderItemIdAsync() =>
            (await response.ReadBodyAsJsonAsync()).Value<int>("orderItemId");
    }
}
