using System;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class CreateOrderRequestBuilder
    {
        private readonly CreateOrderRequest createOrderRequest;

        private CreateOrderRequestBuilder()
        {
            createOrderRequest = new CreateOrderRequest
            {
                Description = "Some Description",
                OrganisationId = Guid.NewGuid(),
                LastUpdatedById = Guid.NewGuid(),
                LastUpdatedByName = "Some user name",
            };
        }

        internal static CreateOrderRequestBuilder Create() => new();

        internal CreateOrderRequestBuilder WithDescription(string description)
        {
            createOrderRequest.Description = description;
            return this;
        }

        internal CreateOrderRequestBuilder WithOrganisationId(Guid organisationId)
        {
            createOrderRequest.OrganisationId = organisationId;
            return this;
        }

        internal CreateOrderRequest Build() => createOrderRequest;
    }
}
