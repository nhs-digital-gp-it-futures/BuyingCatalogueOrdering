using System;
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
        [Test]
        public void WriteRecordsAsync_StreamIsNull_ThrowsArgumentNullException()
        {
            var csvWriter = new AttachmentCsvWriter<CsvHeaderContent, CsvHeaderContentMap>();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await csvWriter.WriteRecordsAsync(null, null));
        }

        [Test]
        public async Task WriteRecordsAsync_RecordsAreWritten_InformationIsCorrect()
        {
            var csvWriter = new AttachmentCsvWriter<CsvHeaderContent, CsvHeaderContentMap>();
            await using var stream = new MemoryStream();

            var expectedDate = DateTime.UtcNow;

            var records = new[] { new CsvHeaderContent {Created = expectedDate, Name = "Bob Builder" }};

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
