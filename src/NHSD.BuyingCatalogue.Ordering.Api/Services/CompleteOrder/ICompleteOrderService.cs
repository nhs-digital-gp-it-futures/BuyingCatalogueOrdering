using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder
{
    public interface ICompleteOrderService
    {
        Task<Result> CompleteAsync(Order order);
    }
}
