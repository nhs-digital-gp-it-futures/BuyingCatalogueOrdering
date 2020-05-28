using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/supplier")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class SupplierSectionController : Controller
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
        public ActionResult Update([FromRoute] [Required] string orderId, [FromBody] [Required] SupplierModel model)
        {
            if (orderId is null)
            {
                return NotFound();
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return NoContent();
        }
    }
}
