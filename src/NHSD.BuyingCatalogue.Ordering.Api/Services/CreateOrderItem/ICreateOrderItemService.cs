using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public interface ICreateOrderItemService
    {
        Task<Result<int>> CreateAsync(CreateOrderItemRequest createOrderItemRequest);
    }
}
