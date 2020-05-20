using System;
using System.Collections.Generic;
using System.Text;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class CreateOrderRequestBuilder
    {
        private readonly CreateOrderRequest _createOrderRequest;

        private CreateOrderRequestBuilder()
        {
            _createOrderRequest = new CreateOrderRequest()
            {
                Description = "Some Description",
                OrganisationId = Guid.NewGuid()
            };            
        }

        internal static CreateOrderRequestBuilder Create() => new CreateOrderRequestBuilder();

        internal CreateOrderRequestBuilder WithDescription(string description)
        {
            _createOrderRequest.Description = description;
            return this;
        }

        internal CreateOrderRequestBuilder WithOrganisationId(Guid organisationId)
        {
            _createOrderRequest.OrganisationId = organisationId;
            return this;
        }

        internal CreateOrderRequest Build() => _createOrderRequest;

    }
}
