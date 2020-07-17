using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class UpdateOrderItemRequest
    {
        private const int MaximumDeliveryDateOffsetDays = 1282;

        private readonly Request _request;
        private readonly string _createOrderItemUrl;

        private static readonly IDictionary<string, Func<UpdateOrderItemRequestPayload>> PayloadFactory =
            new Dictionary<string, Func<UpdateOrderItemRequestPayload>>()
            {
                {
                    "complete", () =>
                    UpdateOrderItemRequestPayloadBuilder
                        .Create()
                        .Build()
                },
                {
                    "high-boundary", () =>
                    UpdateOrderItemRequestPayloadBuilder
                        .Create()
                        .WithPrice(999999999999999.999m)
                        .WithQuantity(int.MaxValue - 1)
                        .Build()
                },
                {
                    "low-boundary", () =>
                    UpdateOrderItemRequestPayloadBuilder
                        .Create()
                        .WithPrice(0)
                        .WithQuantity(1)
                        .Build()
                },
                {
                    "missing-delivery-date", () =>
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithDeliveryDate(null)
                            .Build()
                },
                {
                    "missing-quantity", () =>
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithQuantity(null)
                            .Build()
                },
                {
                    "missing-estimation-period", () =>
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithEstimationPeriod(null)
                            .Build()
                },
                {
                    "missing-price", () =>
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithPrice(null)
                            .Build()
                },
                {
                    "outside-delivery-window", () =>
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithDeliveryDate(new DateTime(2021, 1, 1).AddDays(MaximumDeliveryDateOffsetDays))
                            .Build()
                },
                {
                    "less-than-min-quantity", () => 
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithQuantity(0)
                            .Build()
                },
                {
                    "greater-than-max-quantity", () => 
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithQuantity(int.MaxValue)
                            .Build()
                },
                {
                    "less-than-min-price", () => 
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithPrice(-1)
                            .Build()
                },
                {
                    "greater-than-max-price", () => 
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithPrice(1000000000000000m)
                            .Build()
                },
                {
                    "invalid-value-estimation-period", () =>
                        UpdateOrderItemRequestPayloadBuilder
                            .Create()
                            .WithEstimationPeriod(TimeUnit.Invalid)
                            .Build()
                }
            };

        public UpdateOrderItemRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId,
            int orderItemId)
        {
            if (string.IsNullOrWhiteSpace(orderingApiBaseAddress))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(orderingApiBaseAddress));

            _request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            OrderItemId = orderItemId;

            _createOrderItemUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/order-items/{orderItemId}";
        }

        public string OrderId { get; set; }

        public int OrderItemId { get; set; }

        public UpdateOrderItemRequestPayload Payload { get; set; }

        public async Task ExecuteAsync()
        {
            await _request.PutJsonAsync(_createOrderItemUrl, new
            {
                Payload.DeliveryDate,
                Payload.Quantity,
                EstimationPeriod = Payload.EstimationPeriod?.ToString(),
                Payload.Price
            });
        }

        public void SetPayload(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (!PayloadFactory.TryGetValue(key, out var factory))
                Assert.Fail("Unexpected update order item payload type.");

            Payload = factory();
        }

        public void AssertPayload(OrderItemEntity actual, OrderItemEntity original)
        {
            var expectedEstimationPeriod = Payload.EstimationPeriod;

            if (original.ProvisioningType != ProvisioningType.OnDemand)
            {
                expectedEstimationPeriod = original.EstimationPeriod;
            }

            var expected = new
            {
                OrderId,
                OrderItemId,
                DeliveryDate = Payload.DeliveryDate?.Date,
                EstimationPeriod = expectedEstimationPeriod,
                Payload.Price
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }

    internal sealed class UpdateOrderItemRequestPayload
    {
        public DateTime? DeliveryDate { get; set; }

        public int? Quantity { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public decimal? Price { get; set; }
    }
}
