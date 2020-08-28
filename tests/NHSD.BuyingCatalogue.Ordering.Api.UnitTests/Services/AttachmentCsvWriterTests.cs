using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class AttachmentCsvWriterTests
    {
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void WriteRecordsAsync_StreamIsNull_ThrowsArgumentNullException(bool hasStream, bool hasRecords)
        {
            var csvWriter = new CsvStreamWriter<CsvHeaderContent, CsvHeaderContentMap>();

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await csvWriter.WriteRecordsAsync(hasStream ? new MemoryStream() : null,
                    hasRecords ? new List<CsvHeaderContent>() : null));
        }

        [Test]
        public async Task WriteRecordsAsync_RecordsAreWritten_InformationIsCorrect()
        {
            var csvWriter = new CsvStreamWriter<CsvHeaderContent, CsvHeaderContentMap>();
            await using var stream = new MemoryStream();

            var expectedDate = DateTime.UtcNow;

            var records = new[] { new CsvHeaderContent { Created = expectedDate, Name = "Bob Builder" } };

            await csvWriter.WriteRecordsAsync(stream, records);

            var attachmentData = new MemoryStream(stream.ToArray());
            var csvText = new TextFieldParser(attachmentData);
            csvText.SetDelimiters(",");

            string[] csvHeader = { "Name", "Created Date" };
            string[] expectedData = { "Bob Builder", expectedDate.ToString("dd/MM/yyyy") };

            csvText.ReadFields().Should().BeEquivalentTo(csvHeader);
            csvText.EndOfData.Should().BeFalse();

            csvText.ReadFields().Should().BeEquivalentTo(expectedData);
            csvText.EndOfData.Should().BeTrue();
        }

        private sealed class CsvHeaderContent
        {
            public string Name { get; set; }
            public DateTime? Created { get; set; }
        }

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
