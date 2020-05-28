using System;
using System.Net.Mime;
using System.Linq;
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

            var result = new OrderingPartyModel
            {
                Organisation = new OrganisationModel
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
                    }
                },
                PrimaryContact = primaryContact is null ? null : new PrimaryContactModel
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
        public async Task<ActionResult> Update(string orderId, OrderingPartyModel model)
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

            var primaryContact = model.PrimaryContact;

            var contact = new Contact
            {
                FirstName = primaryContact.FirstName,
                LastName = primaryContact.LastName,
                Email = primaryContact.EmailAddress,
                Phone = primaryContact.TelephoneNumber
            };

            order.OrganisationContact = contact;

            var organisationModel = model.Organisation;
            var addressModel = organisationModel.Address;
            var address = new Address
            {
                Line1 = addressModel.Line1,
                Line2 = addressModel.Line2,
                Line3 = addressModel.Line3,
                Line4 = addressModel.Line4,
                Line5 = addressModel.Line5,
                Town = addressModel.Town,
                County = addressModel.County,
                Postcode = addressModel.Postcode,
                Country = addressModel.Country
            };

            order.OrganisationName = organisationModel.Name;
            order.OrganisationOdsCode = organisationModel.OdsCode;
            order.OrganisationAddress = address;

            var name = User.Identity.Name;

            order.SetLastUpdatedByName(name);
            order.LastUpdatedBy = User.GetUserId();
            order.LastUpdated = DateTime.UtcNow;

            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
