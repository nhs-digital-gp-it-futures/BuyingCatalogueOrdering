using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/default-delivery-date/{catalogueItemId}")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class DefaultDeliveryDateController : ControllerBase
    {
        private readonly IDefaultDeliveryDateService defaultDeliveryDateService;
        private readonly IDefaultDeliveryDateValidator validator;

        public DefaultDeliveryDateController(
            IDefaultDeliveryDateService defaultDeliveryDateService,
            IDefaultDeliveryDateValidator validator)
        {
            this.defaultDeliveryDateService = defaultDeliveryDateService ?? throw new ArgumentNullException(nameof(defaultDeliveryDateService));
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

            Order order = await defaultDeliveryDateService.GetOrder(callOffId, catalogueItemId);

            if (order is null)
                return NotFound();

            (bool isValid, ErrorsModel errors) = validator.Validate(model, order.CommencementDate);

            if (!isValid)
                return BadRequest(errors);

            // ReSharper disable once PossibleInvalidOperationException (covered by model validation)
            DeliveryDateResult addedOrUpdated = await defaultDeliveryDateService.SetDefaultDeliveryDate(callOffId, catalogueItemId, model.DeliveryDate.Value);

            return addedOrUpdated == DeliveryDateResult.Added
                ? CreatedAtAction(nameof(GetAsync).TrimAsync(), new { callOffId = callOffId.ToString(), catalogueItemId = catalogueItemId.ToString() }, null)
                : Ok();
        }

        [HttpGet]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<DefaultDeliveryDateModel>> GetAsync(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var deliveryDate = await defaultDeliveryDateService.GetDefaultDeliveryDate(callOffId, catalogueItemId);

            if (deliveryDate is null)
                return NotFound();

            return new DefaultDeliveryDateModel { DeliveryDate = deliveryDate };
        }
    }
}
