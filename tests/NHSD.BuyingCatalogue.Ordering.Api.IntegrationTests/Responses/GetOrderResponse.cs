using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderResponse
    {
        private readonly Response _response;

        public GetOrderResponse(Response response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public async Task AssertAsync(OrderEntity orderItem)
        {
            var responseContent = await _response.ReadBodyAsJsonAsync();

            var actual = OrderItemEntityBuilder
                .Create()
                .WithOrderId(orderItem.OrderId)
                .Build();

            actual.Should().BeEquivalentTo(orderItem);
        }
    }
}
