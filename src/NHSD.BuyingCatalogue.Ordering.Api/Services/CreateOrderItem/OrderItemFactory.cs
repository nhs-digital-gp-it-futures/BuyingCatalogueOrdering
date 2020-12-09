using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    internal sealed class OrderItemFactory : IOrderItemFactory
    {
        public OrderItem Create(CreateOrderItemRequest request)
        {
            var catalogueItemType = request.CatalogueItemType;
            var provisioningType = request.ProvisioningType;

            return new OrderItem(
                request.OdsCode,
                request.CatalogueItemId,
                catalogueItemType,
                request.CatalogueItemName,
                request.CatalogueSolutionId,
                provisioningType,
                request.CataloguePriceType,
                CataloguePriceUnit.Create(request.CataloguePriceUnitTierName, request.CataloguePriceUnitDescription),
                request.PriceTimeUnit,
                request.CurrencyCode,
                request.Quantity,
                catalogueItemType.InferEstimationPeriod(provisioningType, request.EstimationPeriod),
                request.DeliveryDate,
                request.Price,
                request.OrderItemId);
        }
    }
}
