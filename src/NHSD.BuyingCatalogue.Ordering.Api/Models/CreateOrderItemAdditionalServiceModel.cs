﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemAdditionalServiceModel : CreateOrderItemModel
    {
        [RequiredWhenProvisioningTypeIn("Declarative", "Patient", ErrorMessage = "TimeUnitRequired")]
        public TimeUnitModel TimeUnit { get; set; }

        [Required(ErrorMessage = "CatalogueSolutionIdRequired")]
        [MaxLength(14, ErrorMessage = "CatalogueSolutionIdTooLong")]
        public string CatalogueSolutionId { get; set; }

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
                Domain.CatalogueItemType.AdditionalService,
                CatalogueItemName,
                CatalogueSolutionId,
                ProvisioningType,
                Type,
                ItemUnit?.Name,
                ItemUnit?.Description,
                TimeUnit?.Name,
                CurrencyCode,
                Quantity.Value,
                EstimationPeriod,
                null,
                Price);
        }
    }
}