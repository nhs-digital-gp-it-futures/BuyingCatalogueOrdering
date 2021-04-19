using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/sections/ordering-party")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class OrderingPartyController : ControllerBase
    {
        private readonly IOrderingPartyService orderingPartyService;
        private readonly IContactDetailsService contactDetailsService;

        public OrderingPartyController(
            IOrderingPartyService orderingPartyService,
            IContactDetailsService contactDetailsService)
        {
            this.orderingPartyService = orderingPartyService ?? throw new ArgumentNullException(nameof(orderingPartyService));
            this.contactDetailsService = contactDetailsService ?? throw new ArgumentNullException(nameof(contactDetailsService));
        }

        [HttpGet]
        public async Task<ActionResult<OrderingPartyModel>> GetAsync(CallOffId callOffId)
        {
            var order = await orderingPartyService.GetOrder(callOffId);

            if (order is null)
                return NotFound();

            if (order.OrderingParty is null)
                return NotFound();

            OrderingPartyModel orderingPartyModel = new OrderingPartyModel(
                order.OrderingParty,
                order.OrderingPartyContact);

            return orderingPartyModel;
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync(CallOffId callOffId, OrderingPartyModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await orderingPartyService.GetOrder(callOffId);

            if (order is null)
                return NotFound();

            var orderingParty = new OrderingParty
            {
                Name = model.Name,
                OdsCode = model.OdsCode,
                Address = contactDetailsService.AddOrUpdateAddress(order.OrderingParty.Address, model.Address),
            };

            Contact contact = contactDetailsService.AddOrUpdatePrimaryContact(
                order.OrderingPartyContact,
                model.PrimaryContact);

            await orderingPartyService.SetOrderingParty(order, orderingParty, contact);

            return NoContent();
        }
    }
}
