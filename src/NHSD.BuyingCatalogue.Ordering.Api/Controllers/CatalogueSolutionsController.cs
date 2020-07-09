using System;
using System.Linq;
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
    public sealed class CatalogueSolutionsController : ControllerBase
    {
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
                            serviceRecipient.OdsCode, StringComparison.OrdinalIgnoreCase))?.Name
                    }
                });

            return new CatalogueSolutionsModel { OrderDescription = order.Description.Value, CatalogueSolutions = catalogueSolutionModel };
        }

        [HttpGet]
        [Route("{orderItemId}")]
        public async Task<ActionResult<GetCatalogueSolutionOrderItemModel>> GetOrderItemAsync(string orderId, int orderItemId)
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

            var orderItem = order.OrderItems.FirstOrDefault(item =>
                item.CatalogueItemType.Equals(CatalogueItemType.Solution) && item.OrderItemId == orderItemId);
            if (orderItem is null)
                return NotFound();

            var serviceRecipients = order.ServiceRecipients;

            return new GetCatalogueSolutionOrderItemModel
            {
                ServiceRecipient = new ServiceRecipientModel
                {
                    OdsCode = orderItem.OdsCode,
                    Name = serviceRecipients.FirstOrDefault(serviceRecipient => string.Equals(orderItem.OdsCode,
                        serviceRecipient.OdsCode, StringComparison.OrdinalIgnoreCase))?.Name
                },
                CatalogueSolutionId = orderItem.CatalogueItemId,
                CatalogueItemName = orderItem.CatalogueItemName,
                CurrencyCode = orderItem.CurrencyCode,
                DeliveryDate = orderItem.DeliveryDate,
                EstimationPeriod = orderItem.EstimationPeriod.Name,
                ItemUnit = new ItemUnitModel { Description = orderItem.CataloguePriceUnit.Description, Name = orderItem.CataloguePriceUnit.Name },
                Price = orderItem.Price,
                ProvisioningType = orderItem.ProvisioningType.Name,
                Quantity = orderItem.Quantity,
                Type = orderItem.CataloguePriceType.Name
            };
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<CreateOrderItemResponseModel>> CreateOrderItemAsync(
            string orderId,
            CreateOrderItemSolutionModel model)
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
                return CreatedAtAction(nameof(GetOrderItemAsync).TrimAsync(), null, new { orderId, orderItemId = createOrderItemResponse.OrderItemId }, createOrderItemResponse);
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
            UpdateOrderItemSolutionModel model)
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
                    model.Quantity),
                orderItem.CatalogueItemType,
                orderItem.ProvisioningType);

            if (result.IsSuccess)
                return NoContent();

            var updateOrderItemResponse =
                new UpdateOrderItemResponseModel
                {
                    Errors = result.Errors.Select(x => new ErrorModel(x.Id, x.Field))
                };

            return BadRequest(updateOrderItemResponse);
        }
    }
}
