using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/sections/description")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class OrderDescriptionController : Controller
    {
        private readonly IOrderDescriptionService orderDescriptionService;

        public OrderDescriptionController(IOrderDescriptionService orderDescriptionService)
        {
            this.orderDescriptionService = orderDescriptionService ?? throw new ArgumentNullException(nameof(orderDescriptionService));
        }

        [HttpGet]
        public async Task<ActionResult<OrderDescriptionModel>> GetAsync(CallOffId callOffId)
        {
            string description = await orderDescriptionService.GetOrderDescription(callOffId);

            return new OrderDescriptionModel { Description = description };
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync([FromRoute] Order order, OrderDescriptionModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (order is null)
                return NotFound();

            await orderDescriptionService.SetOrderDescription(order, model.Description);

            return NoContent();
        }
    }
}
