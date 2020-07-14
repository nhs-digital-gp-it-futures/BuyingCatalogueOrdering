﻿using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem
{
    public interface IUpdateOrderItemService
    {
        Task<Result> UpdateAsync(UpdateOrderItemRequest request, CatalogueItemType catalogueItemType, ProvisioningType provisioningType);
    }
}
