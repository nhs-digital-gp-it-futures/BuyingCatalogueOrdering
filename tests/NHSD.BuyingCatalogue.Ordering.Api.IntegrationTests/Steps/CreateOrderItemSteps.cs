﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class CreateOrderItemSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;
        private CreateOrderItemBaseRequest _createOrderItemRequest;
        private CreateOrderItemResponse _createOrderItemResponse;

        private readonly Dictionary<string, Func<Request, string, string, CreateOrderItemBaseRequest>> _createFunctions = new Dictionary<string, Func<Request, string, string, CreateOrderItemBaseRequest>>
        {
            { "catalogue solution", (request, url, orderId) => new CreateCatalogueSolutionOrderItemRequest(
                    request,
                    url,
                    orderId)
            },
            { "additional service", (request, url, orderId) => new CreateAdditionalServiceOrderItemRequest(
                request,
                url,
                orderId)
        }
        };

        public CreateOrderItemSteps(
            Request request,
            Settings settings)
        {
            _request = request;
            _settings = settings;
        }

        [Given(@"the user creates a request to add a new (.*) order item to the order with ID '(.*)'")]
        public void GivenTheUserCreatesARequestToAddANewCatalogueSolutionOrderItemToTheOrderWithId(string itemType, string orderId)
        {
            _createOrderItemRequest = _createFunctions[itemType](_request, _settings.OrderingApiBaseUrl, orderId);
        }

        [Given(@"the user enters the '(.*)' create order item request payload")]
        public void GivenTheUserEntersPayload(string payloadTypeKey)
        {
            _createOrderItemRequest.SetPayload(payloadTypeKey);
        }

        [When(@"the user sends the create order item request")]
        public async Task WhenTheUserSendsTheRequest()
        {
            _createOrderItemResponse = await _createOrderItemRequest.ExecuteAsync();
        }

        [Then(@"the expected order item is created")]
        public async Task ThenTheExpectedCatalogueSolutionOrderItemIsCreated()
        {
            var orderItem = await OrderItemEntity.FetchByCatalogueItemName(_settings.ConnectionString, _createOrderItemRequest.Payload.CatalogueItemName);
            _createOrderItemRequest.AssertPayload(orderItem);
        }

        [When(@"the response contains the new order item ID")]
        public async Task WhenTheResponseContainsTheNewOrderItemId()
        {
            var orderItem = await OrderItemEntity.FetchByCatalogueItemName(_settings.ConnectionString, _createOrderItemRequest.Payload.CatalogueItemName);
            (await _createOrderItemResponse.GetOrderItemIdAsync()).Should().Be(orderItem.OrderItemId);
        }
    }
}
