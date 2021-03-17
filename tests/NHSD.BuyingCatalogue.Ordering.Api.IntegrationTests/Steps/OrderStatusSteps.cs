using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using FluentAssertions;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Drivers;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using SpecFlow.Assist.Dynamic.PropertyNameFormatting;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderStatusSteps
    {
        private readonly Request request;
        private readonly Settings settings;
        private readonly EmailServerDriver emailServerDriver;

        private UpdateOrderStatusRequest updateOrderStatusRequest;

        public OrderStatusSteps(Request request, Settings settings, EmailServerDriver emailServerDriver)
        {
            this.request = request;
            this.settings = settings;
            this.emailServerDriver = emailServerDriver;
        }

        [Given(@"the user creates a request to update the order status for the order with ID (\d{1,6})")]
        public void GivenTheUserCreatesARequestToUpdateTheStatusForTheOrderWithId(int orderId)
        {
            updateOrderStatusRequest = new UpdateOrderStatusRequest(request, settings.OrderingApiBaseUrl, orderId);
        }

        [Given(@"the user enters the '(.*)' update order status request payload")]
        public void GivenTheUserEntersTheUpdateOrderStatusRequestPayload(string payloadTypeKey)
        {
            updateOrderStatusRequest.SetPayload(payloadTypeKey);
        }

        [When(@"the user sends the update order status request")]
        public async Task WhenTheUserSendsTheUpdateOrderStatusRequest()
        {
            await updateOrderStatusRequest.ExecuteAsync();
        }

        [Then(@"the order status is set correctly")]
        public async Task ThenTheOrderStatusIsSetCorrectly()
        {
            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, updateOrderStatusRequest.OrderId);
            var orderStatus = Enum.Parse<OrderStatus>(updateOrderStatusRequest.Payload.Status);

            order.OrderStatus.Should().Be(orderStatus);
        }

        [Then(@"the order completed date is set")]
        public async Task ThenTheOrderCompletedDateIsSet()
        {
            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, updateOrderStatusRequest.OrderId);
            order.Completed.Should().NotBeNull();
        }

        [Then(@"the '(.*)' attachment contains the following data")]
        public async Task ThenTheAttachmentContainsTheFollowingData(string csvFile, Table table)
        {
            var emails = await emailServerDriver.FindAllEmailsAsync();
            var email = emails[0];
            var attachment = email.Attachments.First(a => a.FileName.Contains(Path.ChangeExtension(csvFile, ".csv"), StringComparison.OrdinalIgnoreCase));
            using var attachmentReader = new StreamReader(new MemoryStream(attachment.AttachmentData.ToArray()));

            using var csvReader = new CsvReader(attachmentReader, CultureInfo.CurrentCulture);
            var csvRecords = csvReader.GetRecords<dynamic>().ToList();

            table.CompareToDynamicSet(csvRecords, new PreservePropertyNameFormatter(), false);
        }

        [Then(@"all attachments use UTF8 encoding")]
        public async Task ThenAllAttachmentsUseUtfEncoding()
        {
            var utf8Preamble = Encoding.UTF8.GetPreamble();
            var emails = await emailServerDriver.FindAllEmailsAsync();

            foreach (var email in emails)
            {
                foreach (var attachment in email.Attachments)
                {
                    using var attachmentReader = new BinaryReader(new MemoryStream(attachment.AttachmentData.ToArray()));
                    var attachmentPreamble = attachmentReader.ReadBytes(utf8Preamble.Length);
                    attachmentPreamble.Should().BeEquivalentTo(utf8Preamble);
                }
            }
        }
    }
}
