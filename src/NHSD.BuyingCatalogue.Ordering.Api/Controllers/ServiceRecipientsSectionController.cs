using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/service-recipients")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class ServiceRecipientsSectionController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetAll(string orderId)
        {
            ServiceRecipientsModel model;

            if (_cannedData.Keys.Contains(orderId))
            {
                model = _cannedData[orderId];
            }
            else
            {
                model = new ServiceRecipientsModel
                {
                    ServiceRecipients = new List<ServiceRecipientModel>
                    {
                        new ServiceRecipientModel
                        {
                            ServiceRecipientId = "Service Rec Id",
                            Name = "Some name",
                            OdsCode = "ODS"
                        }
                    }
                };
            }

            return Ok(model);
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public ActionResult Update(string orderId, ServiceRecipientsModel model)
        {
            _cannedData[orderId] = model ?? throw new ArgumentNullException(nameof(model));

            return NoContent();
        }

        private static readonly Dictionary<string, ServiceRecipientsModel> _cannedData = new Dictionary<string, ServiceRecipientsModel>();
    }
}
