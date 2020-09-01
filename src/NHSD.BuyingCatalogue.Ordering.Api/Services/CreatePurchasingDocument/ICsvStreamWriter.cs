using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public interface ICsvStreamWriter<in T>
    {
        Task WriteRecordsAsync(Stream stream, IEnumerable<T> records);
    }
}
