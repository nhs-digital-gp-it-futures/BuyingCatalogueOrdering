using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.ModelBinding
{
    internal sealed class OrderModelBinder : IModelBinder
    {
        internal const string DefaultModelName = "callOffId";

        private readonly ApplicationDbContext context;

        public OrderModelBinder(ApplicationDbContext context) => this.context = context
            ?? throw new ArgumentNullException(nameof(context));

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            if (string.IsNullOrWhiteSpace(modelName))
                modelName = DefaultModelName;

            var result = bindingContext.ValueProvider.GetValue(modelName);
            if (result == ValueProviderResult.None)
                return;

            bindingContext.ModelState.SetModelValue(modelName, result);
            var value = result.FirstValue;
            if (string.IsNullOrWhiteSpace(value))
                return;

            (bool success, CallOffId callOffId) = CallOffId.Parse(value);
            if (!success)
            {
                bindingContext.ModelState.TryAddModelError(modelName, $"{modelName} must be valid call-off ID");
                return;
            }

            var order = await context.Order
                .Where(o => o.Id == callOffId.Id)
                .SingleOrDefaultAsync();

            bindingContext.Result = ModelBindingResult.Success(order);
        }
    }
}
