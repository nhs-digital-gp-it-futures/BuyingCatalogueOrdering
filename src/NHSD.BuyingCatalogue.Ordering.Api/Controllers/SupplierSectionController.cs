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
    [Route("api/v1/orders/{callOffId}/sections/supplier")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class SupplierSectionController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IContactDetailsService contactDetailsService;

        public SupplierSectionController(ApplicationDbContext context, IContactDetailsService contactDetailsService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.contactDetailsService = contactDetailsService ?? throw new ArgumentNullException(nameof(contactDetailsService));
        }

        [HttpGet]
        public async Task<ActionResult<SupplierModel>> GetAsync(CallOffId callOffId)
        {
            var model = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.Supplier).ThenInclude(s => s.Address)
                .Select(o => new SupplierModel(o.Supplier, o.SupplierContact))
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (model is null)
                return NotFound();

            return model;
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> UpdateAsync(CallOffId callOffId, SupplierModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.Supplier).ThenInclude(s => s.Address)
                .Include(o => o.SupplierContact)
                .SingleOrDefaultAsync();

            if (order is null)
                return NotFound();

            var supplier = order.Supplier ?? await context.Supplier.FindAsync(model.SupplierId) ?? new Supplier
            {
                Id = model.SupplierId,
            };

            supplier.Name = model.Name;
            supplier.Address = contactDetailsService.AddOrUpdateAddress(supplier.Address, model.Address);
            order.SupplierContact = contactDetailsService.AddOrUpdatePrimaryContact(
                order.SupplierContact,
                model.PrimaryContact);

            order.Supplier = supplier;

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
