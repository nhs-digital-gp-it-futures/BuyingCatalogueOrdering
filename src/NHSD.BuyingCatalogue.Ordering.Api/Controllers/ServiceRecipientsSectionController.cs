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
    [Route("api/v1/orders/{callOffId}/sections/service-recipients")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class ServiceRecipientsSectionController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServiceRecipientService serviceRecipientService;

        public ServiceRecipientsSectionController(
            ApplicationDbContext context,
            IServiceRecipientService serviceRecipientService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.serviceRecipientService = serviceRecipientService ?? throw new ArgumentNullException(nameof(serviceRecipientService));
        }

        [HttpGet]
        public async Task<ActionResult<ServiceRecipientsModel>> GetAllAsync(CallOffId callOffId)
        {
            if (!await context.Order.AnyAsync(o => o.Id == callOffId.Id))
                return NotFound();

            var selectedRecipients = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .SelectMany(o => o.SelectedServiceRecipients)
                .OrderBy(r => r.Recipient.Name)
                .Select(r => new ServiceRecipientModel { Name = r.Recipient.Name, OdsCode = r.Recipient.OdsCode })
                .AsNoTracking()
                .ToListAsync();

            if (selectedRecipients.Count == 0)
            {
                selectedRecipients = await context.Order
                    .Where(o => o.Id == callOffId.Id)
                    .SelectMany(o => o.OrderItems)
                    .Where(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution)
                    .SelectMany(o => o.OrderItemRecipients)
                    .Select(r => new ServiceRecipientModel { Name = r.Recipient.Name, OdsCode = r.Recipient.OdsCode })
                    .Distinct()
                    .OrderBy(r => r.Name)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return new ServiceRecipientsModel { ServiceRecipients = selectedRecipients };
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync(CallOffId callOffId, ServiceRecipientsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.Progress)
                .Include(o => o.SelectedServiceRecipients)
                .SingleOrDefaultAsync();

            if (order is null)
                return NotFound();

            var recipients = await serviceRecipientService.AddOrUpdateServiceRecipients(model.ServiceRecipients);

            var serviceRecipients = model.ServiceRecipients.Select(r => new SelectedServiceRecipient
            {
                Recipient = recipients[r.OdsCode],
            }).ToList();

            order.SetSelectedServiceRecipients(serviceRecipients);

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
