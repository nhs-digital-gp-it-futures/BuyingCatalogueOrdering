using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/ordering-party")]
    [ApiController]
    [Produces("application/json")]
    public sealed class OrderingPartyController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderingPartyController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));

        }

        [HttpGet]
        public ActionResult Get(string orderId)
        {
            var result = new OrderingPartyModel
            {
                Organisation = new OrganisationModel
                {
                    Name = $"Hampshire CC with orderId {orderId}",
                    OdsCode = "432432", 
                    Address = new AddressModel
                    {
                        Line1 = "Some address",
                        Line2 = "Some Road",
                        Line3 = "Line 3 address",
                        Line4 = "Another line of address",
                        Line5 = "5th line of address",
                        Town = "A Town",
                        County = "Some county",
                        Postcode = "Some postcode",
                        Country = "A country"
                    }
                },
                PrimaryContact = new PrimaryContactModel
                {
                    FirstName = "bob",
                    LastName = "smith",
                    EmailAddress = "bob.smith@email.com",
                    TelephoneNumber = "4342345223  3434324"
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
                County = addressModel.County,
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
