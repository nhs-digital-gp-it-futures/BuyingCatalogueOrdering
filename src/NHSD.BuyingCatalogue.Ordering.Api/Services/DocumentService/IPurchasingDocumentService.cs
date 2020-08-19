using System.IO;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.DocumentService
{
    public interface IPurchasingDocumentService
    {
        Task Create(Stream stream, Order order);
    }
}
