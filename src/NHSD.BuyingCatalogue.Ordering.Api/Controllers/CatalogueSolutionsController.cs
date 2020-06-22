using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/catalogue-solutions")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class CatalogueSolutionsController : ControllerBase
    {
        private static readonly Dictionary<string, CreateOrderItemModel> CatalogueSolutionOrderItems = new Dictionary<string, CreateOrderItemModel>();

        private readonly IOrderRepository _orderRepository;
        private readonly ICreateOrderItemService _createOrderItemService;

        public CatalogueSolutionsController(
            IOrderRepository orderRepository,
            ICreateOrderItemService createOrderItemService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _createOrderItemService = createOrderItemService ?? throw new ArgumentNullException(nameof(createOrderItemService));
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

            var solutionList = Array.Empty<CatalogueSolutionModel>();
            return new CatalogueSolutionsModel { OrderDescription = order.Description.Value, CatalogueSolutions = solutionList};
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

            var name = User.Identity.Name;
            order.SetLastUpdatedBy(User.GetUserId(), name);

            await _orderRepository.UpdateOrderAsync(order);
            return NoContent();
        }

        [HttpGet]
        [Route("{orderItemId}")]
        public ActionResult<CreateOrderItemModel> GetOrderItem(string orderId, string orderItemId)
        {
            var orderItemKey = GetOrderItemKey(orderId,orderItemId);

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
                SolutionId = orderItemId,
                CurrencyCode = "GBP",
                DeliveryDate = "2020-04-27",
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
        public async Task<ActionResult> CreateOrderItemAsync(
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

            var request = new CreateOrderItemRequest(
                order,
                model?.ServiceRecipient?.OdsCode,
                CatalogueItemType.Solution,
                null);

            await _createOrderItemService.CreateAsync(request);

            return Ok();
        }

        [HttpPut]
        [Route("{orderItemId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public ActionResult UpdateOrderItem(string orderId, string orderItemId, UpdateOrderItemModel updateOrderItemModel)
        {
            if (updateOrderItemModel == null)
            {
                throw new ArgumentNullException(nameof(updateOrderItemModel));
            }

            var orderItemKey = GetOrderItemKey(orderId, orderItemId);
            if (CatalogueSolutionOrderItems.ContainsKey(orderItemKey))
            {
                var item = CatalogueSolutionOrderItems[orderItemKey];
                item.Price = updateOrderItemModel.Price;
                item.Quantity = updateOrderItemModel.Quantity;
                item.DeliveryDate = updateOrderItemModel.DeliveryDate;
                item.EstimationPeriod = updateOrderItemModel.EstimationPeriod;
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        private static string GetOrderItemKey(string orderId, string orderItemId)
        {
            return  $"{orderId}_{orderItemId}";
        }
    }
}
