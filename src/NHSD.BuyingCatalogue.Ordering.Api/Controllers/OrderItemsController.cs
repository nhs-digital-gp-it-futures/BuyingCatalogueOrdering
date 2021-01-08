using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/order-items")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class OrderItemsController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IUpdateOrderItemService updateOrderItemService;
        private readonly ICreateOrderItemService createOrderItemService;

        public OrderItemsController(
            IOrderRepository orderRepository,
            IUpdateOrderItemService updateOrderItemService,
            ICreateOrderItemService createOrderItemService)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.updateOrderItemService = updateOrderItemService ?? throw new ArgumentNullException(nameof(updateOrderItemService));
            this.createOrderItemService = createOrderItemService ?? throw new ArgumentNullException(nameof(createOrderItemService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetOrderItemModel>>> ListAsync(
            string orderId,
            [FromQuery] string catalogueItemType)
        {
            static void ConfigureQuery(IOrderQuery query) => query
                .WithOrderItems()
                .WithServiceInstanceItems()
                .WithServiceRecipients()
                .WithoutTracking();

            var order = await orderRepository.GetOrderByIdAsync(
                orderId,
                ConfigureQuery);

            if (order is null)
            {
                return NotFound();
            }

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            IEnumerable<OrderItem> orderItems = order.OrderItems;

            if (!string.IsNullOrWhiteSpace(catalogueItemType))
            {
                var catalogueItemTypeFromName = OrderingEnums.Parse<CatalogueItemType>(catalogueItemType);
                if (catalogueItemTypeFromName is null)
                {
                    return new List<GetOrderItemModel>();
                }

                orderItems = orderItems.Where(i => i.CatalogueItemType.Equals(catalogueItemTypeFromName));
            }

            var serviceInstanceItems = order.ServiceInstanceItems.ToDictionary(i => i.OrderItemId, i => i.ServiceInstanceId);
            var serviceRecipients = order.ServiceRecipients.ToDictionary(r => r.OdsCode, StringComparer.OrdinalIgnoreCase);
            serviceRecipients.TryAdd(
                order.OrganisationOdsCode,
                new ServiceRecipient { OdsCode = order.OrganisationOdsCode, Name = order.OrganisationName });

            GetOrderItemModel Selector(OrderItem item) => new(
                item,
                serviceRecipients[item.OdsCode],
                serviceInstanceItems.GetValueOrDefault(item.OrderItemId));

            return orderItems
                .OrderBy(i => i.Created)
                .Select(Selector)
                .ToList();
        }

        [HttpGet]
        [Route("{orderItemId}")]
        public async Task<ActionResult<GetOrderItemModel>> GetAsync(string orderId, int orderItemId)
        {
            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound();
            }

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            OrderItem orderItem = order.OrderItems.FirstOrDefault(x => x.OrderItemId == orderItemId);
            if (orderItem is null)
                return NotFound();

            var serviceRecipientDictionary = order.ServiceRecipients.ToDictionary(x => x.OdsCode.ToUpperInvariant());

            serviceRecipientDictionary.TryAdd(order.OrganisationOdsCode.ToUpperInvariant(),
                new ServiceRecipient { OdsCode = order.OrganisationOdsCode, Name = order.OrganisationName });

            serviceRecipientDictionary.TryGetValue(orderItem.OdsCode.ToUpperInvariant(), out var serviceRecipient);

            return new GetOrderItemModel(orderItem, serviceRecipient);
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<CreateOrderItemResponseModel>> CreateOrderItemAsync(
            string orderId,
            CreateOrderItemModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound();
            }

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            var createOrderItemResponse = new CreateOrderItemResponseModel();

            var result = await createOrderItemService.CreateAsync(model.ToRequest(order));

            if (result.IsSuccess)
            {
                createOrderItemResponse.OrderItemId = result.Value;
                return CreatedAtAction(nameof(GetAsync).TrimAsync(), "OrderItems", new { orderId, orderItemId = createOrderItemResponse.OrderItemId }, createOrderItemResponse);
            }

            createOrderItemResponse.Errors = result.Errors.Select(x => new ErrorModel(x.Id, x.Field));
            return BadRequest(createOrderItemResponse);
        }

        [HttpPost("batch")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        [UseValidationProblemDetails]
        public async Task<IActionResult> CreateOrderItemsAsync(
            string orderId,
            IEnumerable<CreateOrderItemModel> model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
                return NotFound();

            var validationResult = await createOrderItemService.CreateAsync(
                order,
                model.Select(item => item.ToRequest(order)).ToList());

            if (validationResult.Success)
                return NoContent();

            foreach ((string key, string errorMessage) in validationResult.ToModelErrors())
                ModelState.AddModelError(key, errorMessage);

            return ValidationProblem(ModelState);
        }

        [HttpPut]
        [Route("{orderItemId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<UpdateOrderItemResponseModel>> UpdateOrderItemAsync(
            string orderId,
            int orderItemId,
            UpdateOrderItemModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
                return NotFound();

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
                return Forbid();

            var orderItem = order.OrderItems.FirstOrDefault(
                item => orderItemId.Equals(item.OrderItemId));

            if (orderItem is null)
                return NotFound();

            // TODO: remove this hack (consider custom model binder or HybridModelBinding package)
            model.OrderItemId = orderItemId;

            var result = await updateOrderItemService.UpdateAsync(
                new UpdateOrderItemRequest(order, model),
                orderItem.CatalogueItemType,
                orderItem.ProvisioningType);

            if (result.IsSuccess)
                return NoContent();

            var updateOrderItemResponse = new UpdateOrderItemResponseModel
            {
                Errors = result.Errors.Select(x => new ErrorModel(x.Id, x.Field)),
            };

            return BadRequest(updateOrderItemResponse);
        }
    }
}
