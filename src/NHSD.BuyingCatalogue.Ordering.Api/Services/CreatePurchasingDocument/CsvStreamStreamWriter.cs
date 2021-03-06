﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    internal sealed class CsvStreamStreamWriter<TEntity, TClassMap> : ICsvStreamWriter<TEntity>
        where TClassMap : ClassMap<TEntity>
    {
        public async Task WriteRecordsAsync(Stream stream, IEnumerable<TEntity> records)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            await using var streamWriter = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
            await using var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);

            var options = new TypeConverterOptions { Formats = new[] { "dd/MM/yyyy" } };
            csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

            csvWriter.Context.RegisterClassMap<TClassMap>();
            await csvWriter.WriteRecordsAsync(records);
        }
    }
}
