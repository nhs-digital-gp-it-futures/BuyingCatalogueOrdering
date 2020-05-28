using System;
using System.Net.Mime;
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
    [Route("api/v1/orders/{orderId}/sections/supplier")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class SupplierSectionController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public SupplierSectionController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public async Task<ActionResult<SupplierModel>> GetAsync(string orderId)
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

            var supplierModel = new SupplierModel
            {
                SupplierId = order.SupplierId,
                Name = order.SupplierName,
                Address = order.SupplierAddress.ToModel(),
                PrimaryContact = order.SupplierContact.ToModel()
            };

            return Ok(supplierModel);
        }

       [HttpPut]
        public async Task<ActionResult> UpdateAsync(string orderId, SupplierModel model)
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

            order.SupplierId = model.SupplierId;
            order.SupplierName = model.Name;

            var address = model.Address;

            if(order.SupplierAddress is null)
                order.SupplierAddress = new Address();

            order.SupplierAddress.Line1 = address.Line1;
            order.SupplierAddress.Line2 = address.Line2;
            order.SupplierAddress.Line3 = address.Line3;
            order.SupplierAddress.Line4 = address.Line4;
            order.SupplierAddress.Line5 = address.Line5;
            order.SupplierAddress.Town = address.Town;
            order.SupplierAddress.County = address.County;
            order.SupplierAddress.Postcode = address.Postcode;
            order.SupplierAddress.Country = address.Country;

            if(order.SupplierContact is null)
                order.SupplierContact = new Contact();

            var contact = model.PrimaryContact;

            order.SupplierContact.FirstName = contact.FirstName;
            order.SupplierContact.LastName = contact.LastName;
            order.SupplierContact.Email = contact.EmailAddress;
            order.SupplierContact.Phone = contact.TelephoneNumber;

            var name = User.Identity.Name;
            order.SetLastUpdatedBy(User.GetUserId(), name);

            await _orderRepository.UpdateOrderAsync(order);
            return NoContent();
        }
    }
}
