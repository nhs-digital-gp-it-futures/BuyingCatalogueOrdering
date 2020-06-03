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
        public ActionResult Get(string orderId)
        {
            var model = new ServiceRecipientsModel();

            if (_cannedData.Keys.Contains(orderId))
            {
                model = _cannedData[orderId];
            }
            else
            {
                model.ServiceRecipientId = "Service Rec Id";
                model.Name = "Some name";
                model.OdsCode = "ODS";
            }

            return Ok(model);
        }

        [HttpPut]
        public ActionResult Update(string orderId, ServiceRecipientsModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            _cannedData[orderId] = new ServiceRecipientsModel
            {
                ServiceRecipientId = model.ServiceRecipientId,
                Name = model.Name,
                OdsCode = model.OdsCode
            };
            
            return NoContent();
        }

        private static readonly Dictionary<string, ServiceRecipientsModel> _cannedData = new Dictionary<string, ServiceRecipientsModel>();
    }
}
