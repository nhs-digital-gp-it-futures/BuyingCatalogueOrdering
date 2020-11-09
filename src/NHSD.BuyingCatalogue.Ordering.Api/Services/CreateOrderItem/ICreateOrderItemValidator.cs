using NHSD.BuyingCatalogue.Ordering.Api.Validation;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public interface ICreateOrderItemValidator
    {
        ValidationResult Validate(CreateOrderItemRequest request);
    }
}
