using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class DefaultDeliveryDateController : ControllerBase
    {
        private readonly IDefaultDeliveryDateRepository repository;

        public DefaultDeliveryDateController(IDefaultDeliveryDateRepository repository) =>
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));

        [HttpPut("{orderId}/default-delivery-date/{catalogueItemId}/{priceId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> AddOrUpdateAsync(
            string orderId,
            string catalogueItemId,
            int priceId,
            DefaultDeliveryDateModel defaultDeliveryDate)
        {
            if (defaultDeliveryDate is null)
                throw new ArgumentNullException(nameof(defaultDeliveryDate));

            var isNew = await repository.AddOrUpdateAsync(new DefaultDeliveryDate
            {
                OrderId = orderId,
                CatalogueItemId = catalogueItemId,
                PriceId = priceId,
                DeliveryDate = defaultDeliveryDate.DeliveryDate,
            });

            return isNew ? StatusCode(StatusCodes.Status201Created) : Ok();
        }

        [HttpGet("{orderId}/default-delivery-date/{catalogueItemId}/{priceId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<DefaultDeliveryDateModel>> GetAsync(string orderId, string catalogueItemId, int priceId)
        {
            var date = await repository.GetAsync(orderId, catalogueItemId, priceId);
            if (date is null)
                return NotFound();

            return new DefaultDeliveryDateModel { DeliveryDate = date.DeliveryDate };
        }
    }
}
