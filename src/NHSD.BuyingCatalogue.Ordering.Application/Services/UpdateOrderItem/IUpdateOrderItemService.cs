using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Application.Services.UpdateOrderItem
{
    public interface IUpdateOrderItemService
    {
        Task<Result> UpdateAsync(UpdateOrderItemRequest request);
    }
}
