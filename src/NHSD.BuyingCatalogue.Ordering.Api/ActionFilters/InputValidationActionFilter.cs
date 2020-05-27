using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.ActionFilters
{
    public class InputValidationActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.ModelState.IsValid)
            {
                return;
            }

            var errors = new List<ErrorModel>();
            foreach (var propertyState in context.ModelState)
            {
                errors.AddRange(
                    propertyState.Value.Errors.Select(
                        error => new ErrorModel(propertyState.Key, error.ErrorMessage)));
            }

            context.Result = new JsonResult(new {errors})
            {
                StatusCode = 400
            };
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }
    }
}
