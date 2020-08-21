using System.IO;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public interface ICreatePurchasingDocumentService
    {
        Task CreateDocumentAsync(Stream stream, Order order);
    }
}
