using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;

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
                Description = order.Description.Value
            };

            return Ok(descriptionModel);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync([FromRoute][Required]string orderId, [FromBody][Required] OrderDescriptionModel model)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order is null || model is null)
            {
                return NotFound();
            }

            var isValid = OrderDescription.Create(model.Description);

            if (!isValid.IsSuccess)
            {
                return BadRequest(new ErrorsModel(isValid.Errors.Select(x => new ErrorModel(x.Id, x.Field))));
            }

            order.SetDescription(isValid.Value);

            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
