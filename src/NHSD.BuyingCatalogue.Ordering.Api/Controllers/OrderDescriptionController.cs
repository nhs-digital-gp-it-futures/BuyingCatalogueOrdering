using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/description")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public sealed class OrderDescriptionController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderDescriptionController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync([FromRoute][Required] string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order is null)
            {
                return NotFound();
            }

            var descriptionModel = new OrderDescriptionModel()
            {
                Description = order.Description
            };

            return Ok(descriptionModel);
        }

        [HttpPut]
        public ActionResult Update([FromRoute][Required]string orderId, [FromBody][Required] OrderDescriptionModel model)
        {
            if (orderId is null)
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return NoContent();
        }
    }
}
