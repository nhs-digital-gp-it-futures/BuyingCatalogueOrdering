using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public interface ICreateOrderItemService
    {
        Task<Result<int>> CreateAsync(CreateOrderItemRequest request);

        Task<AggregateValidationResult> CreateAsync(Order order, IReadOnlyList<CreateOrderItemRequest> model);
    }
}
