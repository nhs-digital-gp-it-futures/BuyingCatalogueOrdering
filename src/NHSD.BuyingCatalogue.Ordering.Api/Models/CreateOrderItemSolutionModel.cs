﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemSolutionModel : CreateOrderItemModel
    {
        [Required(ErrorMessage = "DeliveryDateRequired")]
        public DateTime? DeliveryDate { get; set; }

        [RequiredWhenProvisioningTypeIn("Declarative", "Patient", ErrorMessage = "TimeUnitRequired")]
        public TimeUnitModel TimeUnit { get; set; }

        public override CreateOrderItemRequest ToRequest(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));
            if (Quantity == null)
                throw new InvalidOperationException($"Model {nameof(Quantity)} should never be null at this point");

            return new CreateOrderItemRequest(
                order,
                ServiceRecipient?.OdsCode,
                CatalogueItemId,
                Domain.CatalogueItemType.Solution, 
                CatalogueItemName,
                null,
                ProvisioningType,
                Type,
                ItemUnit?.Name,
                ItemUnit?.Description,
                TimeUnit?.Name,
                CurrencyCode,
                Quantity.Value,
                EstimationPeriod,
                DeliveryDate,
                Price);
        }
    }
}
