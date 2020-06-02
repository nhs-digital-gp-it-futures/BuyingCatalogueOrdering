using System;
using System.ComponentModel.DataAnnotations;
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
    [Route("api/v1/orders/{orderId}/sections/description")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
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

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            var descriptionModel = new OrderDescriptionModel()
            {
                Description = order.Description.Value
            };

            return Ok(descriptionModel);
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync([FromRoute][Required]string orderId, [FromBody][Required] OrderDescriptionModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

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

            var isValid = OrderDescription.Create(model.Description);

            if (!isValid.IsSuccess)
            {
                return BadRequest(new ErrorsModel(isValid.Errors.Select(x => new ErrorModel(x.Id, x.Field))));
            }

            order.SetDescription(isValid.Value);
            
            var name = User.Identity.Name;
            order.SetLastUpdatedBy(User.GetUserId(), name);

            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
