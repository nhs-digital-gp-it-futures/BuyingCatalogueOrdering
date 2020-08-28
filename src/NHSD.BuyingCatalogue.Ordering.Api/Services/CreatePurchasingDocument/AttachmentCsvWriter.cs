using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public sealed class AttachmentCsvWriter<T, T2> : IAttachmentCsvWriter<T> where T2 : ClassMap<T>
    {
        public async Task WriteRecordsAsync(Stream stream, IEnumerable<T> records)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            await using var streamWriter = new StreamWriter(stream, leaveOpen: true);
            await using var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);

            var options = new TypeConverterOptions { Formats = new[] { "dd/MM/yyyy" } };
            csvWriter.Configuration.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

            csvWriter.Configuration.RegisterClassMap<T2>();
            await csvWriter.WriteRecordsAsync(records);
        }
    }
}
