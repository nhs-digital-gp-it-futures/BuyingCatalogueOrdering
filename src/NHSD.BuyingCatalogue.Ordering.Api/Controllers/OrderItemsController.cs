using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
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

        public OrderItemsController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
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
