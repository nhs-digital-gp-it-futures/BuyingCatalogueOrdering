using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/commencement-date")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class CommencementDateController : Controller
    {
        private readonly IOrderRepository orderRepository;

        public CommencementDateController(IOrderRepository orderRepository)
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

            var result = new CommencementDateModel { CommencementDate = order.CommencementDate };

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> Update(string orderId, CommencementDateModel model)
        {
            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound();
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

            var name = User.GetUserName();
            order.SetLastUpdatedBy(User.GetUserId(), name);

            await orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
