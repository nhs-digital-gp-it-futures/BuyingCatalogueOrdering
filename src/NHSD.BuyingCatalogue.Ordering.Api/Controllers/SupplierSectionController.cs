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
    [Route("api/v1/orders/{callOffId}/sections/supplier")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class SupplierSectionController : ControllerBase
    {
        private readonly ISupplierSectionService supplierSectionService;
        private readonly IContactDetailsService contactDetailsService;

        public SupplierSectionController(ISupplierSectionService supplierSectionService, IContactDetailsService contactDetailsService)
        {
            this.supplierSectionService = supplierSectionService ?? throw new ArgumentNullException(nameof(supplierSectionService));
            this.contactDetailsService = contactDetailsService ?? throw new ArgumentNullException(nameof(contactDetailsService));
        }

        [HttpGet]
        public async Task<ActionResult<SupplierModel>> GetAsync(CallOffId callOffId)
        {
            var order = await supplierSectionService.GetOrder(callOffId);

            if (order is null)
                return NotFound();

            return new SupplierModel(order.Supplier, order.SupplierContact);
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> UpdateAsync(CallOffId callOffId, SupplierModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await supplierSectionService.GetOrder(callOffId);

            if (order is null)
                return NotFound();

            var supplierModel = new Supplier
            {
                Id = model.SupplierId,
                Name = model.Name,
                Address = contactDetailsService.AddOrUpdateAddress(order.Supplier?.Address, model.Address),
            };

            var contact = contactDetailsService.AddOrUpdatePrimaryContact(
                order.SupplierContact,
                model.PrimaryContact);

            await supplierSectionService.SetSupplierSection(order, supplierModel, contact);

            return NoContent();
        }
    }
}
