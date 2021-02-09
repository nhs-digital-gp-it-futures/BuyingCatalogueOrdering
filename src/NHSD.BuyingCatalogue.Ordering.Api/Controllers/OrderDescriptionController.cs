using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/description")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class OrderDescriptionController : Controller
    {
        private readonly IOrderRepository orderRepository;

        public OrderDescriptionController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(string orderId)
        {
            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound();
            }

            var descriptionModel = new OrderDescriptionModel
            {
                Description = order.Description.Value,
            };

            return Ok(descriptionModel);
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync(string orderId, OrderDescriptionModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound();
            }

            var isValid = OrderDescription.Create(model.Description);

            if (!isValid.IsSuccess)
            {
                return BadRequest(new ErrorsModel(isValid.Errors.Select(d => new ErrorModel(d.Id, d.Field))));
            }

            order.SetDescription(isValid.Value);

            var name = User.GetUserName();
            order.SetLastUpdatedBy(User.GetUserId(), name);

            await orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
