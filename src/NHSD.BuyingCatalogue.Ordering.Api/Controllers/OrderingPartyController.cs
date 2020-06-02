using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/ordering-party")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class OrderingPartyController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderingPartyController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public async Task<ActionResult<OrderingPartyModel>> GetAsync(string orderId)
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

            var organisationAddress = order.OrganisationAddress;
            var primaryContact = order.OrganisationContact;

            if (primaryContact is null)
                return Ok();

            var result = new OrderingPartyModel
            {
                Name = order.OrganisationName,
                OdsCode = order.OrganisationOdsCode,
                Address = organisationAddress is null ? null : new AddressModel
                {
                    Line1 = organisationAddress.Line1,
                    Line2 = organisationAddress.Line2,
                    Line3 = organisationAddress.Line3,
                    Line4 = organisationAddress.Line4,
                    Line5 = organisationAddress.Line5,
                    Town = organisationAddress.Town,
                    County = organisationAddress.County,
                    Postcode = organisationAddress.Postcode,
                    Country = organisationAddress.Country
                },
                PrimaryContact = new PrimaryContactModel
                {
                    FirstName = primaryContact.FirstName,
                    LastName = primaryContact.LastName,
                    EmailAddress = primaryContact.Email,
                    TelephoneNumber = primaryContact.Phone
                }
            };

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync(string orderId, OrderingPartyModel model)
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

            order.OrganisationName = model.Name;
            order.OrganisationOdsCode = model.OdsCode;
            order.OrganisationContact = order.OrganisationContact.FromModel(model.PrimaryContact);
            order.OrganisationAddress = order.OrganisationAddress.FromModel(model.Address);

            var name = User.Identity.Name;
            order.SetLastUpdatedBy(User.GetUserId(), name);

            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
