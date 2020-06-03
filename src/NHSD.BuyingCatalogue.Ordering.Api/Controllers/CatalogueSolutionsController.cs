﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/catalogue-solutions")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class CatalogueSolutionsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            var solution = new CatalogueSolutionModel {Name = "The Big Kahuna"};
            var model = new[] { solution };
            return Ok(model);
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public ActionResult Update()
        {
            return NoContent();
        }
    }
}
