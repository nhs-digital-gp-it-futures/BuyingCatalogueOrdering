using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        [Route("/api/v1/organisations/{organisationId}/[controller]")]
        public async Task<ActionResult> GetAllAsync(Guid organisationId)
        {
            var orders = await _orderRepository.ListOrdersByOrganisationIdAsync(organisationId);

            var orderModelResult = orders.Select(order => new OrderModel
                {
                OrderId = order.OrderId,
                Description = order.Description.Value,
                LastUpdatedBy = order.LastUpdatedByName,
                LastUpdated = order.LastUpdated,
                DateCreated = order.Created,
                Status = order.OrderStatus.Name
            })
                .ToList();

            return Ok(orderModelResult);
        }

        [HttpGet]
        [Route("{orderId}/summary")]
        public async Task<ActionResult> GetOrderSummaryAsync(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order is null)
            {
                return NotFound();
            }

            return Ok(new OrderSummaryModel
            {
                OrderId = orderId,
                OrganisationId = order.OrganisationId,
                Description = order.Description.Value,
                Sections = new List<SectionModel>
                {
                    new SectionModel
                    {
                        Id = "description",
                        Status = "complete"
                    },
                    new SectionModel
                    {
                        Id = "ordering-party",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "supplier",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "commencement-date",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "associated-services",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "service-recipients",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "catalogue-solutions",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "additional-services",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "funding-source",
                        Status = "incomplete"
                    }
                }
            });
        }

        [HttpPost]
        public ActionResult<CreateOrderResponseModel> CreateOrderAsync([FromBody][Required] CreateOrderModel order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            var createOrderResponse = new CreateOrderResponseModel {OrderId = "C0000014-01" };
            return Ok(createOrderResponse);
        }
    }
}
