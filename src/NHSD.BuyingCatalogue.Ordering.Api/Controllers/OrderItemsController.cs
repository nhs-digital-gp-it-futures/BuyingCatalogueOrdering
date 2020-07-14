using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
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
        private readonly IOrderRepository _orderRepository;
        private readonly ICreateOrderItemService _createOrderItemService;

        public OrderItemsController(IOrderRepository orderRepository, ICreateOrderItemService createOrderItemService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _createOrderItemService = createOrderItemService ?? throw new ArgumentNullException(nameof(createOrderItemService));
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<CreateOrderItemResponseModel>> CreateOrderItemAsync(
            string orderId,
            CreateOrderItemBaseModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var order = await _orderRepository.GetOrderByIdAsync(orderId);
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

            var result = await _createOrderItemService.CreateAsync(model.ToRequest(order));

            if (result.IsSuccess)
            {
                createOrderItemResponse.OrderItemId = result.Value;
                return CreatedAtAction(nameof(CatalogueSolutionsController.GetOrderItemAsync).TrimAsync(), "CatalogueSolutions", new { orderId, orderItemId = createOrderItemResponse.OrderItemId }, createOrderItemResponse);
            }

            createOrderItemResponse.Errors = result.Errors.Select(x => new ErrorModel(x.Id, x.Field));
            return BadRequest(createOrderItemResponse);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetOrderItemModel>>> GetAllAsync(string orderId, [FromQuery] string catalogueItemType)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
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
                var catalogueItemTypeFromName = CatalogueItemType.FromName(catalogueItemType);
                if (catalogueItemTypeFromName is null)
                {
                    return new List<GetOrderItemModel>();
                }

                orderItems = orderItems.Where(y => y.CatalogueItemType.Equals(catalogueItemTypeFromName));
            }

            var serviceRecipients = order.ServiceRecipients;

            return orderItems
                .OrderBy(x => x.Created)
                .Select(orderItem => new GetOrderItemModel
                {
                    ItemId = $"{orderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                    ServiceRecipient = new ServiceRecipientModel
                    {
                        Name = serviceRecipients.FirstOrDefault(serviceRecipient => string.Equals(orderItem.OdsCode,
                            serviceRecipient.OdsCode, StringComparison.OrdinalIgnoreCase))?.Name,
                        OdsCode = orderItem.OdsCode
                    },
                    CataloguePriceType = orderItem.CataloguePriceType.Name,
                    CatalogueItemType = orderItem.CatalogueItemType.Name,
                    CatalogueItemName = orderItem.CatalogueItemName,
                    CatalogueItemId = orderItem.CatalogueItemId,
                }).ToList();
        }
    }
}
