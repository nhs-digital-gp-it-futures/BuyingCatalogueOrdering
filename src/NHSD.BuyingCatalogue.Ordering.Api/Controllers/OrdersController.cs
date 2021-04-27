using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class OrdersController : ControllerBase
    {
        private readonly IOrderService orderService;

        private readonly IDictionary<OrderStatus, Func<Order, Task<Result>>> updateOrderStatusActionFactory
            = new Dictionary<OrderStatus, Func<Order, Task<Result>>>();

        public OrdersController(
            IOrderService orderService,
            ICompleteOrderService completeOrderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

            if (completeOrderService is null)
                throw new ArgumentNullException(nameof(completeOrderService));

            updateOrderStatusActionFactory.Add(OrderStatus.Complete, completeOrderService.CompleteAsync);
        }

        [HttpGet]
        [Route("{callOffId}")]
        public async Task<ActionResult<OrderModel>> GetAsync(CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            if (order is null)
                return NotFound();

            return OrderModel.Create(order);
        }

        [HttpGet]
        [Route("/api/v1/organisations/{organisationId}/[controller]")]
        [TypeFilter(typeof(OrganisationIdOrganisationAuthorizationFilter))]
        public async Task<ActionResult<IList<OrderListItemModel>>> GetAllAsync(Guid organisationId)
        {
            var orderingList = await orderService.GetOrderList(organisationId);
            return orderingList.Select(o => new OrderListItemModel(o)).ToList();
        }

        [HttpGet]
        [Route("{callOffId}/summary")]
        public async Task<ActionResult<OrderSummaryModel>> GetOrderSummaryAsync(CallOffId callOffId)
        {
            var order = await orderService.GetOrderSummary(callOffId);

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

            var order = await orderService.CreateOrder(model.Description, model.OrganisationId.Value);

            return CreatedAtAction(
                nameof(GetAsync).TrimAsync(),
                new { callOffId = order.CallOffId.ToString() },
                new { orderId = order.CallOffId.ToString() });
        }

        // ReSharper disable once RouteTemplates.RouteParameterIsNotPassedToMethod (bound to order parameter)
        // ReSharper disable once RouteTemplates.MethodMissingRouteParameters
        [HttpDelete("{callOffId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> DeleteOrderAsync([FromRoute] Order order)
        {
            if (order is null || order.IsDeleted)
                return NotFound();

            await orderService.DeleteOrder(order);

            return NoContent();
        }

        [HttpPut]
        [Route("{callOffId}/status")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<ErrorResponseModel>> UpdateStatusAsync(CallOffId callOffId, StatusModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await orderService.GetOrderCompletedStatus(callOffId);

            if (order is null)
                return NotFound();

            var orderStatus = OrderStatus.FromName(model.Status);
            if (orderStatus is null || !updateOrderStatusActionFactory.TryGetValue(orderStatus, out var updateOrderStatusAsync))
            {
                return BadRequest(new ErrorResponseModel
                {
                    Errors = new[] { ErrorMessages.InvalidOrderStatus() },
                });
            }

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
