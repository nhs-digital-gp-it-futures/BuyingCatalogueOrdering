using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public interface ICreateOrderItemService
    {
        Task CreateAsync(CreateOrderItemRequest createOrderItemRequest);
    }
}
