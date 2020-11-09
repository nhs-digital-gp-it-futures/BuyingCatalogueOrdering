using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;

namespace NHSD.BuyingCatalogue.Ordering.Api.ActionFilters
{
    internal sealed class InputValidationActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (context.ModelState.IsValid)
                return;

            // TODO: Remove filter so that all endpoints return standard validation response (problem details) and remove attribute
            // (requires coordination with FE)
            if (context.ActionDescriptor.EndpointMetadata.OfType<UseValidationProblemDetailsAttribute>().Any())
            {
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
                return;
            }

            var errors = new List<ErrorModel>();
            foreach ((string key, ModelStateEntry entry) in context.ModelState)
            {
                errors.AddRange(
                    entry.Errors.Select(error => new ErrorModel(error.ErrorMessage, key.Split(".").Last())));
            }

            context.Result = new JsonResult(new { errors }) { StatusCode = 400 };
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
