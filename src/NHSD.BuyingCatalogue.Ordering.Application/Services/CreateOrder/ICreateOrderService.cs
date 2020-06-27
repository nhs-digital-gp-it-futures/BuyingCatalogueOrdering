using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Application.Services.CreateOrder
{
    public interface ICreateOrderService
    {
        Task<Result<string>> CreateAsync(CreateOrderRequest createBuyerRequest);
    }
}
