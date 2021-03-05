using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.ModelBinding
{
    internal sealed class OrderModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context) =>
            context.Metadata.ModelType == typeof(Order)
                ? new BinderTypeModelBinder(typeof(OrderModelBinder))
                : null;
    }
}
