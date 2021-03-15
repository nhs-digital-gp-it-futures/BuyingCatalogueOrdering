using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/order-items")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class OrderItemsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ICreateOrderItemService createOrderItemService;

        public OrderItemsController(
            ApplicationDbContext context,
            ICreateOrderItemService createOrderItemService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.createOrderItemService = createOrderItemService ?? throw new ArgumentNullException(nameof(createOrderItemService));
        }

        [HttpGet]
        public async Task<ActionResult<List<GetOrderItemModel>>> ListAsync(
            CallOffId callOffId,
            [FromQuery] CatalogueItemType? catalogueItemType)
        {
            if (!await context.Order.AnyAsync(o => o.Id == callOffId.Id))
                return NotFound();

            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = catalogueItemType is null
                ? o => o.OrderItems
                : o => o.OrderItems.Where(i => i.CatalogueItem.CatalogueItemType == catalogueItemType.Value);

            return await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.PricingUnit)
                .SelectMany(orderItems)
                .OrderBy(i => i.CatalogueItem.Name)
                .Select(i => new GetOrderItemModel(i))
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet]
        [Route("{catalogueItemId}")]
        public async Task<ActionResult<GetOrderItemModel>> GetAsync(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = o =>
                o.OrderItems.Where(i => i.CatalogueItem.Id == catalogueItemId);

            var model = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.PricingUnit)
                .SelectMany(orderItems)
                .Select(i => new GetOrderItemModel(i))
                .SingleOrDefaultAsync();

            if (model is null)
                return NotFound();

            return model;
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

            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.DefaultDeliveryDates)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients)
                .Include(o => o.Progress)
                .SingleOrDefaultAsync();

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
            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = o =>
                o.OrderItems.Where(i => i.CatalogueItem.Id == catalogueItemId);

            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(orderItems)
                .ThenInclude(i => i.CatalogueItem)
                .SingleOrDefaultAsync();

            if (order is null || order.IsDeleted)
            {
                return NotFound();
            }

            if (!order.DeleteOrderItem(catalogueItemId))
            {
                return NotFound();
            }

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
