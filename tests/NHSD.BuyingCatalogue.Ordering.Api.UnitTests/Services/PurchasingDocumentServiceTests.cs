using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class PurchasingDocumentServiceTests
    {
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void CreateDocumentAsync_ArgumentsAreNull_ReturnNullArgumentException(bool hasStream, bool hasOrder)
        {
            var purchasingDocumentService = new PurchasingDocumentService();

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                purchasingDocumentService.CreateDocumentAsync(
                    hasStream ? new MemoryStream() : null,
                    hasOrder ? OrderBuilder.Create().Build() : null));
        }

        [Test]
        public async Task CreateDocumentAsync_DocumentIsCreated_StreamIsAsExpected()
        {
            var purchasingDocumentService = new PurchasingDocumentService();

            await using var stream = new MemoryStream();
            var order = OrderBuilder.Create().Build();
            await purchasingDocumentService.CreateDocumentAsync(stream, OrderBuilder.Create().Build());

            var expectedStreamContent = stream.ToArray();
            var actualStreamContent = Encoding.UTF8.GetBytes($"Call off Party Id{Environment.NewLine}{order.OrderId}{Environment.NewLine}");

            expectedStreamContent.Should().BeEquivalentTo(actualStreamContent);
        }
    }
}
