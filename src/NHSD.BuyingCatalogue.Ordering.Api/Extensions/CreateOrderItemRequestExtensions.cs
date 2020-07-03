using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class CreateOrderItemModelExtensions
    {
        public static CreateOrderItemRequest ToRequest(
            this CreateOrderItemSolutionModel model, 
            Order order, 
            CatalogueItemType catalogueItemType)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (order is null)
                throw new ArgumentNullException(nameof(order));
            if(model.Quantity == null)
                throw new ArgumentException("Model Quantity should never be null at this point", nameof(model));

            return new CreateOrderItemRequest(
                order,
                model.ServiceRecipient?.OdsCode,
                model.CatalogueSolutionId,
                catalogueItemType,
                model.CatalogueSolutionName,
                model.ProvisioningType,
                model.Type,
                model.ItemUnit?.Name,
                model.ItemUnit?.Description,
                model.TimeUnit,
                model.CurrencyCode,
                model.Quantity.Value,
                model.EstimationPeriod,
                model.DeliveryDate,
                model.Price);
        }
    }
}
