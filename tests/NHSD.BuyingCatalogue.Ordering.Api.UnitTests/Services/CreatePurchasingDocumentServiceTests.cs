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
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void Constructor_NullParameter_ThrowsArgumentNullException(bool hasPatientCsvWriter, bool hasPriceTypeCsvWriter)
        {
            var context = CreatePurchasingDocumentServiceTestContext.Setup();

            var builder = CreatePurchasingDocumentServiceBuilder
                .Create()
                .WithPatientNumbersCsvWriter(hasPatientCsvWriter ? context.PatientCsvWriterMock.Object : null)
                .WithPriceTypeCsvWriter(hasPriceTypeCsvWriter ? context.PriceCsvWriterMock.Object : null);

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

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void CreatePriceTypeCsvAsync_ArgumentsAreNull_ReturnNullArgumentException(bool hasStream, bool hasOrder)
        {
            var context = CreatePurchasingDocumentServiceTestContext.Setup();

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                context.CreatePurchasingDocumentService.CreatePriceTypeCsvAsync(
                    hasStream ? new MemoryStream() : null,
                    hasOrder ? OrderBuilder.Create().Build() : null));
        }

        [Test]
        public async Task CreatePriceTypeCsvAsync_DocumentIsCreated_WriteRecordsAsyncCalledOnce()
        {
            var context = CreatePurchasingDocumentServiceTestContext.Setup();
            await using var stream = new MemoryStream();
            var order = OrderBuilder.Create().Build();

            await context.CreatePurchasingDocumentService.CreatePriceTypeCsvAsync(stream, order);

            context.PriceCsvWriterMock.Verify(
                x => x.WriteRecordsAsync(It.IsAny<Stream>(), It.IsAny<IEnumerable<PriceType>>()),
                Times.Once);
        }
    }

    internal sealed class CreatePurchasingDocumentServiceTestContext
    {
        private CreatePurchasingDocumentServiceTestContext()
        {
            PatientCsvWriterMock = new Mock<ICsvStreamWriter<PatientNumbersPriceType>>();
            PatientCsvWriterMock.Setup(x =>
                x.WriteRecordsAsync(It.IsAny<Stream>(), It.IsAny<IEnumerable<PatientNumbersPriceType>>()));

            PriceCsvWriterMock = new Mock<ICsvStreamWriter<PriceType>>();
            PriceCsvWriterMock.Setup(x => x.WriteRecordsAsync(It.IsAny<Stream>(), It.IsAny<IEnumerable<PriceType>>()));

            CreatePurchasingDocumentService = CreatePurchasingDocumentServiceBuilder
                .Create()
                .WithPatientNumbersCsvWriter(PatientCsvWriterMock.Object)
                .WithPriceTypeCsvWriter(PriceCsvWriterMock.Object)
                .Build();
        }

        internal Mock<ICsvStreamWriter<PatientNumbersPriceType>> PatientCsvWriterMock { get; }

        internal Mock<ICsvStreamWriter<PriceType>> PriceCsvWriterMock { get; }

        internal CreatePurchasingDocumentService CreatePurchasingDocumentService { get; }

        public static CreatePurchasingDocumentServiceTestContext Setup() =>
            new CreatePurchasingDocumentServiceTestContext();
    }
}
