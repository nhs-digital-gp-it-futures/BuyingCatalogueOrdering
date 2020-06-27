using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Application.Services.CreateOrderItem
{
    public interface ICreateOrderItemService
    {
        Task<Result<int>> CreateAsync(CreateOrderItemRequest request);
    }
}
