using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/ordering-party")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class OrderingPartyController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderingPartyController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(string orderId)
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

            var contact = model.PrimaryContact;
            var isContactValid = OrderPartyContact.Create(contact.FirstName, contact.LastName, contact.EmailAddress, contact.TelephoneNumber);

            if (!isContactValid.IsSuccess)
            {
                return BadRequest(new ErrorsModel(isContactValid.Errors.Select(x => new ErrorModel(x.Id, x.Field))));
            }

            order.OrganisationContact = isContactValid.Value.GetContact();

            var organisation = model.Organisation;
            var addressModel = organisation.Address;
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
            var isOrganisationValid = OrderPartyOrganisation.Create(organisation.Name, organisation.OdsCode, address);

            if (!isOrganisationValid.IsSuccess)
            {
                return BadRequest(new ErrorsModel(isContactValid.Errors.Select(x => new ErrorModel(x.Id, x.Field))));
            }

            var updateOrganisation = isOrganisationValid.Value.GetOrganisation();

            order.OrganisationName = updateOrganisation.Name;
            order.OrganisationOdsCode = updateOrganisation.OdsCode;
            order.OrganisationAddress = updateOrganisation.Adress;

            var name = User.Identity.Name;

            order.SetLastUpdatedByName(name);
            order.LastUpdatedBy = User.GetUserId();
            order.LastUpdated = DateTime.UtcNow;

            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
