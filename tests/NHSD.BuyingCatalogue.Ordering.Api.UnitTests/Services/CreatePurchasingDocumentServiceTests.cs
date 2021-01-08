using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreatePurchasingDocumentServiceTests
    {
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public static void Constructor_NullParameter_ThrowsArgumentNullException(bool hasPatientCsvWriter, bool hasPriceTypeCsvWriter)
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
        public static void CreatePatientNumbersCsvAsync_ArgumentsAreNull_ReturnNullArgumentException(bool hasStream, bool hasOrder)
        {
            var context = CreatePurchasingDocumentServiceTestContext.Setup();

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                context.CreatePurchasingDocumentService.CreatePatientNumbersCsvAsync(
                    hasStream ? new MemoryStream() : null,
                    hasOrder ? OrderBuilder.Create().Build() : null));
        }

        [Test]
        public static async Task CreatePatientNumbersCsvAsync_DocumentIsCreated_WriteRecordsAsyncCalledOnce()
        {
            var context = CreatePurchasingDocumentServiceTestContext.Setup();
            await using var stream = new MemoryStream();
            var order = OrderBuilder.Create().Build();

            await context.CreatePurchasingDocumentService.CreatePatientNumbersCsvAsync(stream, order);

            context.PatientCsvWriterMock.Verify(
                x => x.WriteRecordsAsync(It.IsAny<Stream>(), It.IsAny<IEnumerable<OdooPatientNumbersOrderItem>>()),
                Times.Once);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public static void CreatePriceTypeCsvAsync_ArgumentsAreNull_ReturnNullArgumentException(bool hasStream, bool hasOrder)
        {
            var context = CreatePurchasingDocumentServiceTestContext.Setup();

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                context.CreatePurchasingDocumentService.CreateCsvAsync(
                    hasStream ? new MemoryStream() : null,
                    hasOrder ? OrderBuilder.Create().Build() : null));
        }

        [Test]
        public static async Task CreatePriceTypeCsvAsync_DocumentIsCreated_WriteRecordsAsyncCalledOnce()
        {
            var context = CreatePurchasingDocumentServiceTestContext.Setup();
            await using var stream = new MemoryStream();
            var order = OrderBuilder.Create().Build();

            await context.CreatePurchasingDocumentService.CreateCsvAsync(stream, order);

            context.PriceCsvWriterMock.Verify(
                x => x.WriteRecordsAsync(It.IsAny<Stream>(), It.IsAny<IEnumerable<OdooOrderItem>>()),
                Times.Once);
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreatePriceTypeCsvAsync_InvokesCreateCsvAsync(
            [Frozen] Mock<ICsvStreamWriter<OdooOrderItem>> csvWriter,
            CreatePurchasingDocumentService service,
            Order order)
        {
            var stream = Mock.Of<Stream>();

            await service.CreateCsvAsync(stream, order);

            csvWriter.Verify(
                c => c.WriteRecordsAsync(It.Is<Stream>(s => s == stream), It.IsNotNull<IEnumerable<OdooOrderItem>>()));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreatePriceTypeCsvAsync_CreatesExpectedPriceType(
            [Frozen] Mock<ICsvStreamWriter<OdooOrderItem>> csvWriter,
            CreatePurchasingDocumentService service,
            OdsOrganisation odsOrganisation)
        {
            var fixture = new Fixture();
            fixture.Customize(new SingleServiceRecipientOrderCustomization(odsOrganisation));

            var item = fixture.Freeze<OrderItem>();
            var order = fixture.Create<Order>();

            IReadOnlyList<OdooOrderItem> actualOrderItems = null;
            void SaveOrderItems(Stream s, IEnumerable<OdooOrderItem> orderItems) => actualOrderItems = orderItems.ToList();

            csvWriter
                .Setup(c => c.WriteRecordsAsync(It.IsNotNull<Stream>(), It.IsNotNull<IEnumerable<OdooOrderItem>>()))
                .Callback<Stream, IEnumerable<OdooOrderItem>>(SaveOrderItems);

            await service.CreateCsvAsync(Mock.Of<Stream>(), order);

            actualOrderItems.Should().NotBeNull();
            actualOrderItems.Should().HaveCount(1);
            actualOrderItems
                .First()
                .Should()
                .BeEquivalentTo(new OdooOrderItem(order, item, odsOrganisation.Name));
        }
    }

    internal sealed class CreatePurchasingDocumentServiceTestContext
    {
        private CreatePurchasingDocumentServiceTestContext()
        {
            PatientCsvWriterMock = new Mock<ICsvStreamWriter<OdooPatientNumbersOrderItem>>();
            PatientCsvWriterMock.Setup(x =>
                x.WriteRecordsAsync(It.IsAny<Stream>(), It.IsAny<IEnumerable<OdooPatientNumbersOrderItem>>()));

            PriceCsvWriterMock = new Mock<ICsvStreamWriter<OdooOrderItem>>();
            PriceCsvWriterMock.Setup(x => x.WriteRecordsAsync(It.IsAny<Stream>(), It.IsAny<IEnumerable<OdooOrderItem>>()));

            CreatePurchasingDocumentService = CreatePurchasingDocumentServiceBuilder
                .Create()
                .WithPatientNumbersCsvWriter(PatientCsvWriterMock.Object)
                .WithPriceTypeCsvWriter(PriceCsvWriterMock.Object)
                .Build();
        }

        internal Mock<ICsvStreamWriter<OdooPatientNumbersOrderItem>> PatientCsvWriterMock { get; }

        internal Mock<ICsvStreamWriter<OdooOrderItem>> PriceCsvWriterMock { get; }

        internal CreatePurchasingDocumentService CreatePurchasingDocumentService { get; }

        public static CreatePurchasingDocumentServiceTestContext Setup() => new();
    }
}
