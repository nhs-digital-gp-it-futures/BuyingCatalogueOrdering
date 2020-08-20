using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.DocumentService;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
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

            Stream stream = null;
            Order order = null;

            if(hasStream)
                stream = new MemoryStream();
            if (hasOrder)
                order = OrderBuilder.Create().Build();

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                purchasingDocumentService.CreateDocumentAsync(stream, order));
        }

        [Test]
        public async Task CreateDocumentAsync_DocumentIsCreated_StreamIsAsExpected()
        {
            var purchasingDocumentService = new PurchasingDocumentService();

            var stream = new MemoryStream();
            var order = OrderBuilder.Create().Build();
            await purchasingDocumentService.CreateDocumentAsync(stream, OrderBuilder.Create().Build());

            var expectedContent = $"Call off Party Id\r\n{order.OrderId}\r";
            var streamBytes = stream.ToArray();
            var contentBytes = Encoding.UTF8.GetBytes(expectedContent);

            streamBytes.Should().BeEquivalentTo(contentBytes);
        }
    }
}
