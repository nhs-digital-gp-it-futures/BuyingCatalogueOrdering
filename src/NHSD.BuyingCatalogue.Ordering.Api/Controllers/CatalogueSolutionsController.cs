using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/catalogue-solutions")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class CatalogueSolutionsController : ControllerBase
    {
        static private readonly Dictionary<string, OrderItemModel> CatalogueSolutionOrderItems = new Dictionary<string, OrderItemModel>();

        private readonly IOrderRepository _orderRepository;
        
        public CatalogueSolutionsController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
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
        public ActionResult<OrderItemModel> GetOrderItem(string orderId, string orderItemId)
        {
            var orderItemKey = $"{orderId}_{orderItemId}";

            if (CatalogueSolutionOrderItems.ContainsKey(orderItemKey))
            {
                return CatalogueSolutionOrderItems[orderItemKey];
            }
            else
            {
                return new OrderItemModel
                {
                    serviceRecipientModel = new ServiceRecipientModel
                    {
                        OdsCode = "OX3"
                    },
                    CatalogueItemId = orderItemId,
                    CurrencyCode = "GBP",
                    DeliverDate = "2020-04-27",
                    EstimationPeriod = "month",
                    ItemUnit = new ItemUnit { Description = "per consultation", Name = "consultation" },
                    Price = 0.1m,
                    ProvisioningType = "OnDemand",
                    Quantity = 3,
                    Type = "flat"
                };
            }
        }

        [HttpPut]
        [Route("{orderItemId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public ActionResult UpdateOrderItem(string orderId, string orderItemId,UpdateOrderItemModel updateOrderItemModel)
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
                item.DeliverDate = updateOrderItemModel.DeliverDate;
                item.EstimationPeriod = updateOrderItemModel.EstimationPeriod;
            }
            return NoContent();
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public ActionResult CreateOrderItem(string orderId,OrderItemModel orderItemModel)
        {
            if (orderItemModel == null)
            {
                throw new ArgumentNullException(nameof(orderItemModel));
            }

            CatalogueSolutionOrderItems[GetOrderItemKey(orderId, orderItemModel.CatalogueItemId)] = orderItemModel;

            return NoContent();
        }

        public static string GetOrderItemKey(string orderId, string orderItemId)
        {
            return  $"{orderId}_{orderItemId}";
        }
    }
}
