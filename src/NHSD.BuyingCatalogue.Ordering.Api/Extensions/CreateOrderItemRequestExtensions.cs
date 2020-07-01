using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class CreateOrderItemModelExtensions
    {
        public static CreateOrderItemRequest ToRequest(
            this CreateOrderItemModel model, 
            Order order, 
            CatalogueItemType catalogueItemType)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            return new CreateOrderItemRequest(
                order,
                model.ServiceRecipient?.OdsCode,
                model.CatalogueSolutionId,
                catalogueItemType,
                model.CatalogueSolutionName,
                model.ProvisioningType,
                model.Type,
                model.ItemUnitModel?.Name,
                model.ItemUnitModel?.Description,
                null,
                model.CurrencyCode,
                model.Quantity,
                model.EstimationPeriod,
                model.DeliveryDate,
                model.Price);
        }
    }
}
