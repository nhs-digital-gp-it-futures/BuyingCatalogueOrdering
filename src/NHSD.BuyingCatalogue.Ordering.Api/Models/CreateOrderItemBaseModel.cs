﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    [JsonConverter(typeof(CreateOrderItemBaseModelConverter))]
    public class CreateOrderItemBaseModel
    {
        [Required(ErrorMessage = "CatalogueItemTypeRequired")]
        [RegularExpression("Solution|AdditionalService|AssociatedService", ErrorMessage = "CatalogueItemTypeValidValue")]
        public string CatalogueItemType { get; set; }

        public virtual CreateOrderItemRequest ToRequest(Order order) { throw new NotImplementedException(); }
    }
}
