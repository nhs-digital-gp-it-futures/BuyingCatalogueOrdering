using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/sections/ordering-party")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class OrderingPartyController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IContactDetailsService contactDetailsService;

        public OrderingPartyController(
            ApplicationDbContext context,
            IContactDetailsService contactDetailsService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.contactDetailsService = contactDetailsService ?? throw new ArgumentNullException(nameof(contactDetailsService));
        }

        [HttpGet]
        public async Task<ActionResult<OrderingPartyModel>> GetAsync(CallOffId callOffId)
        {
            var orderingPartyModel = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty).ThenInclude(p => p.Address)
                .Select(o => new OrderingPartyModel(o.OrderingParty, o.OrderingPartyContact))
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (orderingPartyModel is null)
                return NotFound();

            return orderingPartyModel;
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync(CallOffId callOffId, OrderingPartyModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty).ThenInclude(p => p.Address)
                .Include(o => o.OrderingPartyContact)
                .SingleOrDefaultAsync();

            if (order is null)
                return NotFound();

            var orderingParty = order.OrderingParty;
            orderingParty.Name = model.Name;
            orderingParty.OdsCode = model.OdsCode;

            orderingParty.Address = contactDetailsService.AddOrUpdateAddress(orderingParty.Address, model.Address);
            order.OrderingPartyContact = contactDetailsService.AddOrUpdatePrimaryContact(
                order.OrderingPartyContact,
                model.PrimaryContact);

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
