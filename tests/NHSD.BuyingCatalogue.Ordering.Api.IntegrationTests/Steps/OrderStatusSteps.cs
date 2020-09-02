using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Drivers;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal class OrderStatusSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;
        private readonly OrderContext _orderContext;
        private readonly EmailServerDriver _emailServerDriver;

        private UpdateOrderStatusRequest _updateOrderStatusRequest;

        public OrderStatusSteps(Request request, Settings settings, OrderContext orderContext, EmailServerDriver emailServerDriver)
        {
            _request = request;
            _settings = settings;
            _orderContext = orderContext;
            _emailServerDriver = emailServerDriver;
        }

        [Given(@"the user creates a request to update the order status for the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToUpdateTheStatusForTheOrderWithId(string orderId)
        {
            _updateOrderStatusRequest = new UpdateOrderStatusRequest(_request, _settings.OrderingApiBaseUrl, orderId);
        }

        [Given(@"the user enters the '(.*)' update order status request payload")]
        public void GivenTheUserEntersTheUpdateOrderStatusRequestPayload(string payloadTypeKey)
        {
            _updateOrderStatusRequest.SetPayload(payloadTypeKey);
        }

        [When(@"the user sends the update order status request")]
        public async Task WhenTheUserSendsTheUpdateOrderStatusRequest()
        {
            await _updateOrderStatusRequest.ExecuteAsync();
        }

        [Then(@"the order status is set correctly")]
        public async Task ThenTheOrderStatusIsSetCorrectly()
        {
            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _updateOrderStatusRequest.OrderId);
            Enum.TryParse<OrderStatus>(_updateOrderStatusRequest.Payload.Status, out var orderStatus);

            order.OrderStatus.Should().Be(orderStatus);
        }

        [Then(@"the order completed date is set")]
        public async Task ThenTheOrderCompletedDateIsSet()
        {
            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, _updateOrderStatusRequest.OrderId);
            order.Completed.Should().NotBeNull();
        }
        
        [Then(@"the price type attachment contains the correct information")]
        public async Task ThenThePriceTypeAttachmentContainsTheCorrectInformation()
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
                "Unit Time",
                "Estimation Period",
                "Price",
                "Order Type",
                "Funding Type",
                "M1 planned (Delivery Date)",
                "Actual M1 date",
                "Buyer verification date (M2)",
                "Cease Date"
            };

            var actual = (await _emailServerDriver.FindAllEmailsAsync()).First().Attachments
                .First(x => x.FileName.Contains("Full.csv"));

            await using var attachmentData = new MemoryStream(actual.AttachmentData.ToArray());
            var csvText = new TextFieldParser(attachmentData);
            csvText.SetDelimiters(",");

            csvText.ReadFields().Should().BeEquivalentTo(patientNumbersCsvHeader);

            var order = _orderContext.OrderReferenceList.GetByOrderId(_updateOrderStatusRequest.OrderId);
            var orderItems = _orderContext.OrderItemReferenceList.FindByOrderId(_updateOrderStatusRequest.OrderId).ToArray();
            var serviceRecipients = _orderContext.ServiceRecipientReferenceList.FindByOrderId(_updateOrderStatusRequest.OrderId).ToList();

            foreach (var orderItem in orderItems)
            {
                csvText.EndOfData.Should().BeFalse();

                var row = csvText.ReadFields();

                var data = CreatePriceTypeAttachmentData(order, orderItem, serviceRecipients);

                row.Should().BeEquivalentTo(data);
            }

            csvText.EndOfData.Should().BeTrue();
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

            var actual = (await _emailServerDriver.FindAllEmailsAsync()).First().Attachments
                .First(x => x.FileName.Contains("Patients.csv"));

            await using var attachmentData = new MemoryStream(actual.AttachmentData.ToArray());
            var csvText = new TextFieldParser(attachmentData);
            csvText.SetDelimiters(",");

            csvText.ReadFields().Should().BeEquivalentTo(patientNumbersCsvHeader);

            var order = _orderContext.OrderReferenceList.GetByOrderId(_updateOrderStatusRequest.OrderId);
            var orderItems = _orderContext.OrderItemReferenceList.FindByOrderId(_updateOrderStatusRequest.OrderId).ToArray();
            var serviceRecipients = _orderContext.ServiceRecipientReferenceList.FindByOrderId(_updateOrderStatusRequest.OrderId).ToList();

            foreach (var orderItem in orderItems)
            {
                csvText.EndOfData.Should().BeFalse();

                var row = csvText.ReadFields();

                var data = CreatePatientNumbersTypeAttachmentData(order, orderItem, serviceRecipients);

                row.Should().BeEquivalentTo(data);
            }

            csvText.EndOfData.Should().BeTrue();
        }

        private string[] CreatePriceTypeAttachmentData(OrderEntity order, OrderItemEntity orderItem, IEnumerable<ServiceRecipientEntity> serviceRecipientDictionary)
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
                $"{order.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                order.SupplierId,
                order.SupplierName,
                orderItem.CatalogueItemId,
                orderItem.CatalogueItemName,
                orderItem.CatalogueItemType.ToString(),
                orderItem.Quantity.ToString(),
                orderItem.PricingUnitDescription,
                orderItem.TimeUnit.GetValueOrDefault().ToDescription(), 
                orderItem.EstimationPeriod.GetValueOrDefault().ToDescription(),
                orderItem.Price.GetValueOrDefault() == 0 ? 0.ToString() : orderItem.Price.GetValueOrDefault().ToString("#.000"),
                ((int)orderItem.ProvisioningType).ToString(),
                "Central",
                orderItem.DeliveryDate?.ToString("dd/MM/yyyy"),
                String.Empty,
                String.Empty,
                String.Empty
            };

            return data;
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
                $"{order.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
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
                String.Empty,
                String.Empty,
                String.Empty
            };

            return data;
        }
    }
}
