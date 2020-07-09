using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderItemResponse
    {
        private readonly Response _response;

        public GetOrderItemResponse(Response response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public async Task AssertAsync(IEnumerable<OrderItemEntity> expectedOrderItems, IEnumerable<ServiceRecipientEntity> expectedServiceRecipients, string catalogueItemType)
        {
            var responseContext = await _response.ReadBodyAsJsonAsync();

            const string serviceRecipientToken = "serviceRecipient";

            var orderItems = responseContext.Select(orderItem => new
            {
                ItemId = orderItem.Value<string>("itemId"),
                ServiceRecipientName = orderItem.SelectToken(serviceRecipientToken).Value<string>("name"),
                ServiceRecipientOdsCode = orderItem.SelectToken(serviceRecipientToken).Value<string>("odsCode"),
                CataloguePriceType = orderItem.Value<string>("cataloguePriceType"),
                CatalogueItemType = orderItem.Value<string>("catalogueItemType"),
                CatalogueItemName = orderItem.Value<string>("catalogueItemName"),
                CatalogueItemId = orderItem.Value<string>("catalogueItemId")
            });

            var expectedResult = expectedOrderItems.Select(expectedItems => new
            {
                ItemId = $"{expectedItems.OrderId}{expectedItems.OdsCode}{expectedItems.OrderItemId}",
                ServiceRecipientName = expectedServiceRecipients.FirstOrDefault(serviceRecipient => string.Equals(expectedItems.OdsCode,
                    serviceRecipient.OdsCode, StringComparison.InvariantCulture))?.Name,
                ServiceRecipientOdsCode = expectedItems.OdsCode,
                CataloguePriceType = expectedItems.CataloguePriceType.ToString(),
                CatalogueItemType = expectedItems.CatalogueItemType.ToString(),
                expectedItems.CatalogueItemName,
                expectedItems.CatalogueItemId
            });

            if (catalogueItemType != null)
            {
                expectedResult = expectedResult.Where(x => x.CatalogueItemType == catalogueItemType);
            }

            orderItems.Should().BeEquivalentTo(expectedResult);
        }
    }
}
