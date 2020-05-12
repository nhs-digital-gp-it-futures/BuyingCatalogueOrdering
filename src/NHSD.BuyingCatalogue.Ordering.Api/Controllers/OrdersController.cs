using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/organisations/{organisationId}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public sealed class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync(Guid organisationId)
        {
            var orders = await _orderRepository.ListOrdersByOrganisationIdAsync(organisationId);

            var orderModelResult = orders.Select(order => new OrderModel()
            {
                OrderId = order.OrderId,
                Description = order.Description,
                LastUpdatedBy = order.LastUpdatedBy,
                LastUpdated = order.LastUpdated,
                DateCreated = order.Created,
                Status = order.OrderStatus.Name
            })
                .ToList();

            return Ok(orderModelResult);
        }
    }
}
