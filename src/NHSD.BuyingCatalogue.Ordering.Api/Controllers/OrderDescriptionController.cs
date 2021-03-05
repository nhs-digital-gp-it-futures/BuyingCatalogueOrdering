using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/sections/description")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class OrderDescriptionController : Controller
    {
        private readonly ApplicationDbContext context;

        public OrderDescriptionController(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<OrderDescriptionModel>> GetAsync(CallOffId callOffId)
        {
            var model = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Select(o => new OrderDescriptionModel { Description = o.Description })
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (model is null)
                return NotFound();

            return model;
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync([FromRoute] Order order, OrderDescriptionModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (order is null)
                return NotFound();

            order.Description = model.Description;

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
