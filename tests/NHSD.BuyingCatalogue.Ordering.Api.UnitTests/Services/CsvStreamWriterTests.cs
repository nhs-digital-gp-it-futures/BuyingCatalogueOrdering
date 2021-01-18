using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.VisualBasic.FileIO;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CsvStreamWriterTests
    {
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public static void WriteRecordsAsync_StreamIsNull_ThrowsArgumentNullException(bool hasStream, bool hasRecords)
        {
            var csvWriter = new CsvStreamStreamWriter<CsvHeaderContent, CsvHeaderContentMap>();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await csvWriter.WriteRecordsAsync(
                hasStream ? new MemoryStream() : null,
                hasRecords ? new List<CsvHeaderContent>() : null));
        }

        [Test]
        public static async Task WriteRecordsAsync_RecordsAreWritten_InformationIsCorrect()
        {
            var csvWriter = new CsvStreamStreamWriter<CsvHeaderContent, CsvHeaderContentMap>();
            await using var stream = new MemoryStream();

            var expectedDate = DateTime.UtcNow;

            var records = new[] { new CsvHeaderContent { Created = expectedDate, Name = "Bob Builder" } };

            await csvWriter.WriteRecordsAsync(stream, records);

            var attachmentData = new MemoryStream(stream.ToArray());
            using var csvText = new TextFieldParser(attachmentData);
            csvText.SetDelimiters(",");

            string[] csvHeader = { "Name", "Created Date" };
            string[] expectedData = { "Bob Builder", expectedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) };

            csvText.ReadFields().Should().BeEquivalentTo(csvHeader);
            csvText.EndOfData.Should().BeFalse();

            csvText.ReadFields().Should().BeEquivalentTo(expectedData);
            csvText.EndOfData.Should().BeTrue();
        }

        [Test]
        public static async Task WriteRecordsAsync_UsesUtf8Encoding()
        {
            var csvWriter = new CsvStreamStreamWriter<CsvHeaderContent, CsvHeaderContentMap>();
            await using var stream = new MemoryStream();

            var records = new[] { new CsvHeaderContent { Created = DateTime.UtcNow, Name = "Bob Builder" } };

            await csvWriter.WriteRecordsAsync(stream, records);

            var utf8Preamble = Encoding.UTF8.GetPreamble();

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new BinaryReader(stream, Encoding.UTF8);
            var csvPreamble = reader.ReadBytes(utf8Preamble.Length);

            csvPreamble.Should().BeEquivalentTo(utf8Preamble);
        }

        private sealed class CsvHeaderContent
        {
            public string Name { get; init; }

            public DateTime? Created { get; init; }
        }

        [UsedImplicitly]
        [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by CsvHelper")]
        private sealed class CsvHeaderContentMap : ClassMap<CsvHeaderContent>
        {
            public CsvHeaderContentMap()
            {
                Map(x => x.Name).Index(0).Name("Name");
                Map(x => x.Created).Index(1).Name("Created Date");
            }
        }
    }
}
