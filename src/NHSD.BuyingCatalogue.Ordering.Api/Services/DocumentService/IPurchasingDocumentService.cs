using System.IO;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.DocumentService
{
    public interface IPurchasingDocumentService
    {
        Task Create(Order order, Stream stream);
    }
}
