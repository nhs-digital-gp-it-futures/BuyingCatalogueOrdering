using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/order-items")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService orderItemService;
        private readonly ICreateOrderItemService createOrderItemService;

        public OrderItemsController(
            IOrderItemService orderItemService,
            ICreateOrderItemService createOrderItemService)
        {
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.createOrderItemService = createOrderItemService ?? throw new ArgumentNullException(nameof(createOrderItemService));
        }

        [HttpGet]
        public async Task<ActionResult<List<GetOrderItemModel>>> ListAsync(
            CallOffId callOffId,
            [FromQuery] CatalogueItemType? catalogueItemType)
        {
            var order = await orderItemService.GetOrder(callOffId);
            if (order is null)
                return NotFound();

            var orderItems = await orderItemService.GetOrderItems(callOffId, catalogueItemType);
            return orderItems.OrderBy(i => i.CatalogueItem.Name).Select(i => new GetOrderItemModel(i)).ToList();
        }

        [HttpGet]
        [Route("{catalogueItemId}")]
        public async Task<ActionResult<GetOrderItemModel>> GetAsync(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var orderItem = await orderItemService.GetOrderItem(callOffId, catalogueItemId);

            if (orderItem is null)
                return NotFound();

            return new GetOrderItemModel(orderItem);
        }

        [HttpPut("{catalogueItemId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        [UseValidationProblemDetails]
        public async Task<IActionResult> CreateOrderItemAsync(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await orderItemService.GetOrder(callOffId);

            if (order is null)
                return NotFound();

            var validationResult = await createOrderItemService.CreateAsync(order, catalogueItemId, model);

            if (validationResult.Success)
            {
                return CreatedAtAction(
                    nameof(GetAsync).TrimAsync(),
                    new { callOffId = callOffId.ToString(), catalogueItemId = catalogueItemId.ToString() },
                    null);
            }

            foreach ((string key, string errorMessage) in validationResult.ToModelErrors())
                ModelState.AddModelError(key, errorMessage);

            return ValidationProblem(ModelState);
        }

        [Authorize(Policy = PolicyName.CanManageOrders)]
        [HttpDelete("{catalogueItemId}")]
        public async Task<ActionResult> DeleteOrderItemAsync(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId)
        {
            var order = await orderItemService.GetOrderWithCatalogueItem(callOffId, catalogueItemId);
            if (order is null || order.IsDeleted)
            {
                return NotFound();
            }

            if (await orderItemService.DeleteOrderItem(order, catalogueItemId) < 1)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
