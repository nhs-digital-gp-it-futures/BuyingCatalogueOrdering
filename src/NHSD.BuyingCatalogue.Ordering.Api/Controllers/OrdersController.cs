using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        private readonly IDictionary<OrderStatus, Func<Order, Task<Result>>> updateOrderStatusActionFactory
            = new Dictionary<OrderStatus, Func<Order, Task<Result>>>();

        public OrdersController(
            ApplicationDbContext context,
            ICompleteOrderService completeOrderService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));

            if (completeOrderService is null)
                throw new ArgumentNullException(nameof(completeOrderService));

            updateOrderStatusActionFactory.Add(OrderStatus.Complete, completeOrderService.CompleteAsync);
        }

        [HttpGet]
        [Route("{callOffId}")]
        public async Task<ActionResult<OrderModel>> GetAsync(CallOffId callOffId)
        {
            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty).ThenInclude(p => p.Address)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier).ThenInclude(s => s.Address)
                .Include(o => o.SupplierContact)
                .Include(o => o.ServiceInstanceItems)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(o => o.OrderItems).ThenInclude(i => i.PricingUnit)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (order is null)
                return NotFound();

            return OrderModel.Create(order);
        }

        [HttpGet]
        [Route("/api/v1/organisations/{organisationId}/[controller]")]
        [TypeFilter(typeof(OrganisationIdOrganisationAuthorizationFilter))]
        public async Task<ActionResult<IList<OrderListItemModel>>> GetAllAsync(Guid organisationId)
        {
            return await context.OrderingParty
                .Where(o => o.Id == organisationId)
                .SelectMany(o => o.Orders)
                .Select(o => new OrderListItemModel(o))
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet]
        [Route("{callOffId}/summary")]
        public async Task<ActionResult<OrderSummaryModel>> GetOrderSummaryAsync(CallOffId callOffId)
        {
            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.SupplierContact)
                .Include(o => o.SelectedServiceRecipients)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.Progress)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (order is null)
                return NotFound();

            return OrderSummaryModel.Create(order);
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> CreateOrderAsync(CreateOrderModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != model.OrganisationId)
                return Forbid();

            var orderingParty = await context.OrderingParty.FindAsync(model.OrganisationId)
                ?? new OrderingParty { Id = model.OrganisationId.Value };

            var order = new Order
            {
                Description = model.Description,
                OrderingParty = orderingParty,
            };

            context.Add(order);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAsync).TrimAsync(),
                new { callOffId = order.CallOffId.ToString() },
                new { orderId = order.CallOffId.ToString() });
        }

        // {order} is actually a call-off ID here but model binding maps to an Order. However, the route parameter name
        // must match the method parameter name for the model to be considered valid when the ApiController attribute is present.
        [HttpDelete]
        [Route("{order}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> DeleteOrderAsync([FromRoute] Order order)
        {
            if (order is null || order.IsDeleted)
                return NotFound();

            order.IsDeleted = true;

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [Route("{callOffId}/status")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<ErrorResponseModel>> UpdateStatusAsync(CallOffId callOffId, StatusModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var orderStatus = OrderStatus.FromName(model.Status);
            if (orderStatus is null || !updateOrderStatusActionFactory.TryGetValue(orderStatus, out var updateOrderStatusAsync))
            {
                return BadRequest(new ErrorResponseModel
                {
                    Errors = new[] { ErrorMessages.InvalidOrderStatus() },
                });
            }

            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (order is null)
                return NotFound();

            var completeOrderResult = await updateOrderStatusAsync(order);
            if (!completeOrderResult.IsSuccess)
            {
                return BadRequest(new ErrorResponseModel
                {
                    Errors = completeOrderResult.Errors.Select(error => new ErrorModel(error.Id, error.Field)),
                });
            }

            return NoContent();
        }
    }
}
