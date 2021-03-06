﻿using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class SupplierSectionSteps
    {
        private readonly ScenarioContext context;
        private readonly Request request;
        private readonly Response response;
        private readonly Settings settings;

        private readonly string orderSupplierSectionUrl;

        public SupplierSectionSteps(
            ScenarioContext context,
            Request request,
            Response response,
            Settings settings)
        {
            this.context = context;
            this.request = request;
            this.response = response;
            this.settings = settings;

            orderSupplierSectionUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/C{0}-01/sections/supplier";
        }

        [When(@"the user makes a request to retrieve the order supplier section with the ID (\d{1,6})")]
        public async Task WhenTheUserMakesARequestToRetrieveTheOrderSupplierSectionWithId(int orderId)
        {
            await request.GetAsync(string.Format(CultureInfo.InvariantCulture, orderSupplierSectionUrl, orderId));
        }

        [Then(@"the response contains the following supplier details")]
        public async Task ThenTheResponseContainsTheFollowingSupplierDetails(Table table)
        {
            var jsonResponse = await response.ReadBodyAsJsonAsync();

            var actual = new SupplierSectionTable
            {
                SupplierId = jsonResponse.Value<string>("supplierId"),
                SupplierName = jsonResponse.Value<string>("name"),
            };

            var expected = table.CreateInstance<SupplierSectionTable>();
            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the response contains the following primary supplier contact details")]
        public async Task ThenTheResponseContainsTheFollowingSupplierContactDetails(Table table)
        {
            var jsonResponse = await response.ReadBodyAsJsonAsync();

            var primaryContactResponse = jsonResponse.SelectToken("primaryContact");
            Assert.IsNotNull(primaryContactResponse);

            var actual = new SupplierContactTable
            {
                FirstName = primaryContactResponse.Value<string>("firstName"),
                LastName = primaryContactResponse.Value<string>("lastName"),
                Email = primaryContactResponse.Value<string>("emailAddress"),
                Phone = primaryContactResponse.Value<string>("telephoneNumber"),
            };

            var expected = table.CreateInstance<SupplierContactTable>();
            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the response contains the following supplier address")]
        public async Task ThenTheResponseContainsTheFollowingSupplierAddress(Table table)
        {
            var jsonResponse = await response.ReadBodyAsJsonAsync();

            var address = jsonResponse.SelectToken("address");
            Assert.IsNotNull(address);

            var actual = new SupplierAddressTable
            {
                Line1 = address.Value<string>("line1"),
                Line2 = address.Value<string>("line2"),
                Line3 = address.Value<string>("line3"),
                Line4 = address.Value<string>("line4"),
                Line5 = address.Value<string>("line5"),
                Town = address.Value<string>("town"),
                County = address.Value<string>("county"),
                Postcode = address.Value<string>("postcode"),
                Country = address.Value<string>("country"),
            };

            var expected = table.CreateInstance<SupplierAddressTable>();
            actual.Should().BeEquivalentTo(expected);
        }

        [When(@"the user makes a request to update the supplier with order ID (.*)")]
        public async Task WhenTheUserMakesARequestToUpdateTheSupplierWithOrderId(string orderId, Table table)
        {
            var supplierTable = table.CreateInstance<SupplierSectionTable>();

            context.TryGetValue(ScenarioContextKeys.SupplierAddress, out var address);
            context.TryGetValue(ScenarioContextKeys.SupplierContact, out var contact);

            var data = new
            {
                supplierTable.SupplierId,
                Name = supplierTable.SupplierName,
                Address = address,
                PrimaryContact = contact,
            };

            await request.PutJsonAsync(
                string.Format(CultureInfo.InvariantCulture, orderSupplierSectionUrl, orderId),
                data);
        }

        [When(@"the user makes a request to update the supplier with order ID (.*) with no model")]
        public async Task WhenTheUserMakesARequestToUpdateTheSupplierWithOrderIdWithNoModel(string orderId)
        {
            await request.PutJsonAsync(
                string.Format(CultureInfo.InvariantCulture, orderSupplierSectionUrl, orderId),
                null);
        }

        [Then(@"the supplier address for order (\d{1,6}) is")]
        public async Task ThenTheSupplierAddressForOrderIs(int orderId, Table table)
        {
            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            var supplier = await SupplierEntity.FetchById(settings.ConnectionString, order.SupplierId);

            var actual = await AddressEntity.FetchAddressById(settings.ConnectionString, supplier.AddressId);
            var address = table.CreateInstance<SupplierAddressTable>();

            actual.Should().BeEquivalentTo(address);
        }

        [Then(@"the supplier contact for order (.*) is")]
        public async Task ThenTheSupplierContactIdContactForOrderIs(int orderId, Table table)
        {
            var contact = table.CreateInstance<SupplierContactTable>();

            var contactId = (await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId))
                .SupplierContactId;

            var actual = await ContactEntity.FetchContactById(settings.ConnectionString, contactId);
            actual.Should().BeEquivalentTo(contact);
        }

        [Then(@"the supplier for order (\d{1,6}) is updated")]
        public async Task ThenTheSupplierForOrderIsUpdated(int orderId, Table table)
        {
            var expectedSupplier = table.CreateInstance<SupplierSectionTable>();

            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            var supplier = await SupplierEntity.FetchById(settings.ConnectionString, order.SupplierId);

            var actual = new { order.SupplierId, SupplierName = supplier.Name };
            actual.Should().BeEquivalentTo(expectedSupplier);
        }

        [Given(@"the user wants to update the supplier address section")]
        public void WhenTheUserWantsToUpdateTheSupplierAddressSection(Table table)
        {
            var address = table.CreateInstance<SupplierAddressTable>();
            context[ScenarioContextKeys.SupplierAddress] = address;
        }

        [Given(@"the user wants to update the supplier contact section")]
        public void WhenTheUserWantsToUpdateTheSectionForTheContact(Table table)
        {
            var contact = table.CreateInstance<ContactTable>();
            context[ScenarioContextKeys.SupplierContact] = contact;
        }

        private sealed class SupplierSectionTable
        {
            public string SupplierId { get; init; }

            public string SupplierName { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class SupplierContactTable
        {
            public string FirstName { get; init; }

            public string LastName { get; init; }

            public string Email { get; init; }

            public string Phone { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ContactTable
        {
            public string FirstName { get; init; }

            public string LastName { get; init; }

            public string EmailAddress { get; init; }

            public string TelephoneNumber { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class SupplierAddressTable
        {
            public string Line1 { get; init; }

            public string Line2 { get; init; }

            public string Line3 { get; init; }

            public string Line4 { get; init; }

            public string Line5 { get; init; }

            public string Town { get; init; }

            public string County { get; init; }

            public string Postcode { get; init; }

            public string Country { get; init; }
        }
    }
}
