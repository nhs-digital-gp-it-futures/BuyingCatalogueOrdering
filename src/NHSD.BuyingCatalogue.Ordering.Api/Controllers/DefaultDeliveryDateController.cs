using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class DefaultDeliveryDateController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IDefaultDeliveryDateRepository repository;
        private readonly IDefaultDeliveryDateValidator validator;

        public DefaultDeliveryDateController(
            IDefaultDeliveryDateRepository repository,
            IDefaultDeliveryDateValidator validator,
            IOrderRepository orderRepository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

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

            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
                return NotFound();

            (bool isValid, ErrorsModel errors) = validator.Validate(
                defaultDeliveryDate,
                order.CommencementDate);

            if (!isValid)
                return BadRequest(errors);

            var isNew = await repository.AddOrUpdateAsync(new DefaultDeliveryDate
            {
                OrderId = orderId,
                CatalogueItemId = catalogueItemId,
                PriceId = priceId,

                // ReSharper disable once PossibleInvalidOperationException (covered by model validation)
                DeliveryDate = defaultDeliveryDate.DeliveryDate.Value,
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
