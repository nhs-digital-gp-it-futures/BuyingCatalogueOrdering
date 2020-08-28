using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Data;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Drivers;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using static System.String;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class EmailSteps
    {
        private readonly EmailServerDriver _emailServerDriver;
        private readonly OrderContext _orderContext;
        private readonly ScenarioContext _scenarioContext;

        public EmailSteps(EmailServerDriver emailServerDriver, OrderContext orderContext, ScenarioContext scenarioContext)
        {
            _emailServerDriver = emailServerDriver ?? throw new ArgumentNullException(nameof(emailServerDriver));
            _orderContext = orderContext ?? throw new ArgumentNullException(nameof(orderContext));
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
        }

        [Then(@"only one email is sent")]
        public async Task OnlyOneEmailIsSent()
        {
            var emailCount = (await _emailServerDriver.FindAllEmailsAsync()).Count;
            emailCount.Should().Be(1);
        }

        [Then(@"no email is sent")]
        public async Task EmailIsNotSent()
        {
            var emails = await _emailServerDriver.FindAllEmailsAsync();
            emails.Should().BeNullOrEmpty();
        }

        [Then(@"the email contains the following information")]
        public async Task ThenTheEmailContainsTheFollowingInformation(Table table)
        {
            var expectedContents = table.CreateSet<EmailContentsTable>().First();

            var expected = new
            {
                From = new List<EmailAddress> { new EmailAddress(Empty, expectedContents.From) },
                To = new List<EmailAddress> { new EmailAddress(Empty, expectedContents.To) },
                expectedContents.Text,
                expectedContents.Subject,
            };

            var actual = (await _emailServerDriver.FindAllEmailsAsync()).First();

            actual.Should().BeEquivalentTo(expected, conf => conf.IncludingAllDeclaredProperties());
        }

        [Then(@"the patient numbers price type attachment contains the correct information")]
        public async Task ThenThePatientNumbersPriceTypeAttachmentContainsTheCorrectInformation()
        {
            string[] patientNumbersCsvHeader = {
                "Call Off Agreement ID",
                "Call Off Ordering Party ID",
                "Call Off Ordering Party Name",
                "Call Off Commencement Date",
                "Service Recipient ID",
                "Service Recipient Name",
                "Service Recipient Item ID",
                "Supplier ID",
                "Supplier Name",
                "Product ID",
                "Product Name",
                "Product Type",
                "Quantity Ordered",
                "Unit of Order",
                "Price",
                "Funding Type",
                "M1 planned (Delivery Date)",
                "Actual M1 date",
                "Buyer verification date (M2)",
                "Cease Date"
            };

            var actual = (await _emailServerDriver.FindAllEmailsAsync()).First().Attachments.First();

            var attachmentData = new MemoryStream(actual.AttachmentData.ToArray());
            var csvText = new TextFieldParser(attachmentData);
            csvText.SetDelimiters(",");

            csvText.ReadFields().Should().BeEquivalentTo(patientNumbersCsvHeader);

            var orderId = _scenarioContext[ScenarioContextKeys.OrderId].ToString();

            var order = _orderContext.OrderReferenceList.GetByOrderId(orderId);
            var orderItems = _orderContext.OrderItemReferenceList.FindByOrderId(orderId).ToArray();
            var serviceRecipients = _orderContext.ServiceRecipientReferenceList.FindByOrderId(orderId).ToList();

            foreach (var orderItem in orderItems)
            {
                csvText.EndOfData.Should().BeFalse();

                var row = csvText.ReadFields();

                var data = CreatePatientNumbersTypeAttachmentData(order, orderItem, serviceRecipients);

                row.Should().BeEquivalentTo(data);
            }

            csvText.EndOfData.Should().BeTrue();
        }

        private string[] CreatePatientNumbersTypeAttachmentData(OrderEntity order, OrderItemEntity orderItem, IEnumerable<ServiceRecipientEntity> serviceRecipientDictionary)
        {
            string[] data =
            {
                order.OrderId,
                order.OrganisationOdsCode,
                order.OrganisationName,
                order.CommencementDate?.ToString("dd/MM/yyyy"),
                orderItem.OdsCode,
                serviceRecipientDictionary.FirstOrDefault(item =>
                    string.Equals(orderItem.OdsCode,
                        item.OdsCode, StringComparison.OrdinalIgnoreCase))
                    ?.Name,
                $"{orderItem.OrderItemId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                order.SupplierId,
                order.SupplierName,
                orderItem.CatalogueItemId,
                orderItem.CatalogueItemName,
                orderItem.CatalogueItemType.ToString(),
                orderItem.Quantity.ToString(),
                orderItem.PricingUnitDescription,
                orderItem.Price.GetValueOrDefault() == 0 ? 0.ToString() : orderItem.Price.GetValueOrDefault().ToString("#.000"),
                "Central",
                orderItem.DeliveryDate?.ToString("dd/MM/yyyy"),
                Empty,
                Empty,
                Empty
            };

            return data;
        }

        private sealed class EmailContentsTable
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Subject { get; set; }
            public string Text { get; set; }
        }
    }
}
