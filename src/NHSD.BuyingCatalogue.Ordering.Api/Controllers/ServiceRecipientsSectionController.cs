using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/sections/service-recipients")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class ServiceRecipientsSectionController : ControllerBase
    {
        private readonly IServiceRecipientService serviceRecipientService;

        public ServiceRecipientsSectionController(
            IServiceRecipientService serviceRecipientService)
        {
            this.serviceRecipientService = serviceRecipientService ?? throw new ArgumentNullException(nameof(serviceRecipientService));
        }

        [HttpGet]
        public async Task<ActionResult<ServiceRecipientsModel>> GetAllAsync(CallOffId callOffId)
        {
            var selectedRecipients = await serviceRecipientService.GetAllOrderItemRecipients(callOffId);

            if (selectedRecipients is null)
                return NotFound();

            var selectedRecipientsModel = selectedRecipients.Select(r =>
                new ServiceRecipientModel { Name = r.Name, OdsCode = r.OdsCode })
                .ToList();

            return new ServiceRecipientsModel { ServiceRecipients = selectedRecipientsModel };
        }
    }
}
