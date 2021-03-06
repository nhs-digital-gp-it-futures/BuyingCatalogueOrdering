﻿using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class GetOrderItemsRequest
    {
        private readonly Request request;
        private readonly string getOrderItemUrl;

        public GetOrderItemsRequest(
            Request request,
            string orderingApiBaseAddress,
            int orderId,
            string catalogueItemType)
        {
            this.request = request;
            OrderId = orderId;
            CatalogueItemType = catalogueItemType;

            getOrderItemUrl = $"{orderingApiBaseAddress}/api/v1/orders/C{orderId}-01/order-items";

            if (!string.IsNullOrWhiteSpace(CatalogueItemType))
            {
                getOrderItemUrl += $"?catalogueItemType={CatalogueItemType}";
            }
        }

        public int OrderId { get; }

        public string CatalogueItemType { get; }

        public async Task<GetOrderItemsResponse> ExecuteAsync()
        {
            var response = await request.GetAsync(getOrderItemUrl);
            return new GetOrderItemsResponse(response);
        }
    }
}
