using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CreatePurchasingDocumentServiceTests
    {
        [Test]
        public void Constructor_NullParameter_ThrowsArgumentNullException()
        {
            var builder = CreatePurchasingDocumentServiceBuilder
                .Create()
                .WithPatientNumbersCsvWriter(null);

            Assert.Throws<ArgumentNullException>(() => builder.Build());
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void CreatePatientNumbersCsvAsync_ArgumentsAreNull_ReturnNullArgumentException(bool hasStream, bool hasOrder)
        {
            var context = CreatePurchasingDocumentServiceTestContext.Setup();

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                context.CreatePurchasingDocumentService.CreatePatientNumbersCsvAsync(
                    hasStream ? new MemoryStream() : null,
                    hasOrder ? OrderBuilder.Create().Build() : null));
        }

        [Test]
        public async Task CreatePatientNumbersCsvAsync_DocumentIsCreated_WriteRecordsAsyncCalledOnce()
        {
            var context = CreatePurchasingDocumentServiceTestContext.Setup();
            await using var stream = new MemoryStream();
            var order = OrderBuilder.Create().Build();

            await context.CreatePurchasingDocumentService.CreatePatientNumbersCsvAsync(stream, order);

            context.PatientCsvWriterMock.Verify(
                x => x.WriteRecordsAsync(It.IsAny<Stream>(), It.IsAny<IEnumerable<PatientNumbersPriceType>>()),
                Times.Once);
        }
    }

    internal sealed class CreatePurchasingDocumentServiceTestContext
    {
        private CreatePurchasingDocumentServiceTestContext()
        {
            PatientCsvWriterMock = new Mock<IAttachmentCsvWriter<PatientNumbersPriceType>>();
            PatientCsvWriterMock.Setup(x =>
                x.WriteRecordsAsync(It.IsAny<Stream>(), It.IsAny<IEnumerable<PatientNumbersPriceType>>()));

            CreatePurchasingDocumentService = CreatePurchasingDocumentServiceBuilder
                .Create()
                .WithPatientNumbersCsvWriter(PatientCsvWriterMock.Object)
                .Build();
        }

        internal Mock<IAttachmentCsvWriter<PatientNumbersPriceType>> PatientCsvWriterMock;

        internal CreatePurchasingDocumentService CreatePurchasingDocumentService { get; }

        public static CreatePurchasingDocumentServiceTestContext Setup() =>
            new CreatePurchasingDocumentServiceTestContext();
    }
}
