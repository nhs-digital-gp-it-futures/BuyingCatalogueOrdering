using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/catalogue-solutions")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Swagger doesn't allow static functions. Suppression will be removed when the proper implementation is added")]
    public sealed class CatalogueSolutionsController : ControllerBase
    {
        private static readonly Dictionary<string, CreateOrderItemModel> CatalogueSolutionOrderItems = new Dictionary<string, CreateOrderItemModel>();

        private readonly IOrderRepository _orderRepository;
        private readonly ICreateOrderItemService _createOrderItemService;
        private readonly IUpdateOrderItemService _updateOrderItemService;

        public CatalogueSolutionsController(
            IOrderRepository orderRepository,
            ICreateOrderItemService createOrderItemService,
            IUpdateOrderItemService updateOrderItemService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _createOrderItemService = createOrderItemService ?? throw new ArgumentNullException(nameof(createOrderItemService));
            _updateOrderItemService = updateOrderItemService ?? throw new ArgumentNullException(nameof(updateOrderItemService));
        }

        [HttpGet]
        public async Task<ActionResult<CatalogueSolutionsModel>> GetAllAsync(string orderId)
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

            var serviceRecipients = order.ServiceRecipients;
            var catalogueSolutionModel = order.OrderItems.Where(y => y.CatalogueItemType.Equals(CatalogueItemType.Solution))
                .Select(x => new CatalogueSolutionModel
            {
                OrderItemId = x.OrderItemId,
                SolutionName = x.CatalogueItemName,
                ServiceRecipient = new GetServiceRecipientModel
                {
                    OdsCode = x.OdsCode,
                    Name = serviceRecipients.FirstOrDefault(serviceRecipient => string.Equals(x.OdsCode,
                        serviceRecipient.OdsCode, StringComparison.Ordinal))?.Name
                }
            });

            return new CatalogueSolutionsModel { OrderDescription = order.Description.Value, CatalogueSolutions = catalogueSolutionModel };
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync(string orderId)
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

            order.CatalogueSolutionsViewed = true;
            order.SetLastUpdatedBy(User.GetUserId(), User.GetUserName());

            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }

        [HttpGet]
        [Route("{orderItemId}")]
        public ActionResult<CreateOrderItemModel> GetOrderItem(string orderId, string orderItemId)
        {
            var orderItemKey = GetOrderItemKey(orderId, orderItemId);

            if (CatalogueSolutionOrderItems.ContainsKey(orderItemKey))
            {
                return CatalogueSolutionOrderItems[orderItemKey];
            }

            return new CreateOrderItemModel
            {
                ServiceRecipient = new ServiceRecipientModel
                {
                    OdsCode = "OX3"
                },
                CatalogueSolutionId = orderItemId,
                CurrencyCode = "GBP",
                DeliveryDate = DateTime.UtcNow,
                EstimationPeriod = "month",
                ItemUnitModel = new ItemUnitModel { Description = "per consultation", Name = "consultation" },
                Price = 0.1m,
                ProvisioningType = "OnDemand",
                Quantity = 3,
                Type = "flat"
            };
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<CreateOrderItemResponseModel>> CreateOrderItemAsync(
            string orderId,
            CreateOrderItemModel model)
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

            var createOrderItemResponse = new CreateOrderItemResponseModel();

            var result = await _createOrderItemService.CreateAsync(model.ToRequest(order, CatalogueItemType.Solution));
            if (result.IsSuccess)
            {
                createOrderItemResponse.OrderItemId = result.Value;
                return CreatedAtAction(nameof(GetOrderItem).TrimAsync(), null, new { orderId, orderItemId = createOrderItemResponse.OrderItemId }, createOrderItemResponse);
            }

            createOrderItemResponse.Errors = result.Errors.Select(x => new ErrorModel(x.Id, x.Field));
            return BadRequest(createOrderItemResponse);
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

            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
                return NotFound();

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
                return Forbid();

            var orderItem = order.OrderItems.FirstOrDefault(
                item => orderItemId.Equals(item.OrderItemId) 
                        && CatalogueItemType.Solution.Equals(item.CatalogueItemType));

            if (orderItem is null)
                return NotFound();

            var result = await _updateOrderItemService.UpdateAsync(
                new UpdateOrderItemRequest(
                    model.DeliveryDate,
                    model.EstimationPeriod,
                    order,
                    orderItemId,
                    model.Price,
                    model.Quantity));

            if (result.IsSuccess)
                return NoContent();

            var updateOrderItemResponse =
                new UpdateOrderItemResponseModel
                {
                    Errors = result.Errors.Select(x => new ErrorModel(x.Id, x.Field))
                };

            return BadRequest(updateOrderItemResponse);
        }

        private static string GetOrderItemKey(string orderId, string orderItemId)
        {
            return $"{orderId}_{orderItemId}";
        }
    }
}
