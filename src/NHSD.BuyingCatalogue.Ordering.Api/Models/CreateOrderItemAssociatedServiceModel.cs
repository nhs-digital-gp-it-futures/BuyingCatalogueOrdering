using System;
using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemAssociatedServiceModel : CreateOrderItemModel
    {
        public override CreateOrderItemRequest ToRequest(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));
            if (Quantity == null)
                throw new InvalidOperationException($"Model {nameof(Quantity)} should never be null at this point");

            return new CreateOrderItemRequest(
                order,
                order.OrganisationOdsCode,
                CatalogueItemId,
                Domain.CatalogueItemType.AssociatedService,
                CatalogueItemName,
                null,
                ProvisioningType,
                Type,
                ItemUnit?.Name,
                ItemUnit?.Description,
                null,
                CurrencyCode,
                Quantity.Value,
                EstimationPeriod,
                null,
                Price);
        }
    }
}
