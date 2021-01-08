using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class CreateOrderItemResponse
    {
        private readonly Response _response;

        public CreateOrderItemResponse(
            Response response)
        {
            _response = response;
        }

        public async Task<int?> GetOrderItemIdAsync() =>
            (await _response.ReadBodyAsJsonAsync()).Value<int>("orderItemId");
    }
}
