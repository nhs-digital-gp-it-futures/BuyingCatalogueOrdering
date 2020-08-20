using System.IO;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.DocumentService
{
    public interface IPurchasingDocumentService
    {
        Task CreateDocumentAsync(Stream stream, Order order);
    }
}
