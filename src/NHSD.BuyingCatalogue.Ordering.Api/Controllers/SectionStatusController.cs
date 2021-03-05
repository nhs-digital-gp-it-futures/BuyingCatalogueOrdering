using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class SectionStatusController : ControllerBase
    {
        private static readonly Dictionary<string, Action<Order>> CompleteSectionActionsDictionary = new()
        {
            { SectionModel.CatalogueSolutions.Id, o => o.Progress.CatalogueSolutionsViewed = true },
            { SectionModel.AdditionalServices.Id, o => o.Progress.AdditionalServicesViewed = true },
            { SectionModel.AssociatedServices.Id, o => o.Progress.AssociatedServicesViewed = true },
        };

        private readonly ApplicationDbContext context;

        public SectionStatusController(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPut]
        [Route("{callOffId}/sections/{sectionId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> UpdateStatusAsync(
            [FromRoute] Order order,
            string sectionId,
            UpdateOrderSectionModel sectionStatus)
        {
            if (sectionStatus is null)
                throw new ArgumentNullException(nameof(sectionStatus));

            if (sectionId is null)
                throw new ArgumentNullException(nameof(sectionId));

            if (order is null)
                return NotFound();

            if (CompleteSectionActionsDictionary.ContainsKey(sectionId))
            {
                if (sectionStatus.Status == "complete")
                    CompleteSectionActionsDictionary[sectionId](order);
            }
            else
            {
                return Forbid();
            }

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
