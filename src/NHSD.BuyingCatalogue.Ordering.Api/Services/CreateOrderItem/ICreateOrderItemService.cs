using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public interface ICreateOrderItemService
    {
        Task<AggregateValidationResult> CreateAsync(Order order, CatalogueItemId catalogueItemId, CreateOrderItemModel model);
    }
}
