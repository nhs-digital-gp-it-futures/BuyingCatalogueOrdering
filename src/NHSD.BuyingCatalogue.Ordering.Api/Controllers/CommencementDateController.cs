using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/commencement-date")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class CommencementDateController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public CommencementDateController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public ActionResult Get()
        {
            var result = new CommencementDateModel { CommencementDate = DateTime.UtcNow };

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> Update(string orderId, CommencementDateModel model)
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

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!model.CommencementDate.HasValue)
            {
                throw new ArgumentException(nameof(model.CommencementDate));
            }

            order.CommencementDate = model.CommencementDate.Value;
            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
