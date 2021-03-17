using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderItemsResponse : GetOrderItemResponseBase
    {
        private readonly Response response;

        public GetOrderItemsResponse(Response response)
        {
            this.response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public async Task AssertAsync(
            IEnumerable<OrderItemEntity> expectedOrderItems,
            IDictionary<string, ServiceRecipientEntity> serviceRecipients,
            IDictionary<(int OrderId, string CatalogueItemId), IList<OrderItemRecipientEntity>> orderItemRecipients,
            IDictionary<string, PricingUnitEntity> pricingUnits,
            IDictionary<string, CatalogueItemEntity> catalogueItems,
            string catalogueItemType)
        {
            var responseContext = await response.ReadBodyAsJsonAsync();
            var orderItems = responseContext.Select(ReadOrderItem);

            bool CatalogueItemTypeMatches(OrderItemEntity i) =>
                catalogueItemType is null || catalogueItemType.Equals(
                    catalogueItems[i.CatalogueItemId].CatalogueItemType.ToString(),
                    StringComparison.OrdinalIgnoreCase);

            var expectedItems = expectedOrderItems
                .Where(CatalogueItemTypeMatches)
                .OrderBy(expectedItem => catalogueItems[expectedItem.CatalogueItemId].Name)
                .Select(expectedItem => ConvertToExpectedBody(
                    expectedItem,
                    serviceRecipients,
                    orderItemRecipients[(expectedItem.OrderId, expectedItem.CatalogueItemId)],
                    pricingUnits));

            orderItems.Should().BeEquivalentTo(expectedItems, options => options.WithStrictOrdering());
        }
    }
}
