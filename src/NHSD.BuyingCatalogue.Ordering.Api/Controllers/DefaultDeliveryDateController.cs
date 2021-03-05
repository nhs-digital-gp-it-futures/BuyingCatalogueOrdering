using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/default-delivery-date/{catalogueItemId}")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class DefaultDeliveryDateController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IDefaultDeliveryDateValidator validator;

        public DefaultDeliveryDateController(
            ApplicationDbContext context,
            IDefaultDeliveryDateValidator validator)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> AddOrUpdateAsync(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            DefaultDeliveryDateModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catalogueItemId))
                .SingleOrDefaultAsync();

            if (order is null)
                return NotFound();

            (bool isValid, ErrorsModel errors) = validator.Validate(model, order.CommencementDate);

            if (!isValid)
                return BadRequest(errors);

            // ReSharper disable once PossibleInvalidOperationException (covered by model validation)
            DeliveryDateResult addedOrUpdated = order.SetDefaultDeliveryDate(catalogueItemId, model.DeliveryDate.Value);

            await context.SaveChangesAsync();

            return addedOrUpdated == DeliveryDateResult.Added
                ? CreatedAtAction(nameof(GetAsync).TrimAsync(), new { callOffId = callOffId.ToString(), catalogueItemId = catalogueItemId.ToString() }, null)
                : Ok();
        }

        [HttpGet]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<DefaultDeliveryDateModel>> GetAsync(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            Expression<Func<Order, IEnumerable<DefaultDeliveryDate>>> defaultDeliveryDate = o
                => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catalogueItemId);

            var model = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(defaultDeliveryDate)
                .SelectMany(defaultDeliveryDate)
                .Select(d => new DefaultDeliveryDateModel { DeliveryDate = d.DeliveryDate })
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (model is null)
                return NotFound();

            return model;
        }
    }
}
