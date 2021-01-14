using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrderSummaryModelBuilder
    {
        private readonly string description;
        private readonly string status;

        private string orderId;
        private Guid organisationId;
        private IEnumerable<SectionModel> sections;
        private string sectionStatus;

        private OrderSummaryModelBuilder()
        {
            orderId = "C000014-01";
            description = "Some Description";
            organisationId = Guid.NewGuid();
            sections = SectionModelListBuilder.Create().Build();
            sectionStatus = "incomplete";
            status = OrderStatus.Incomplete.Name;
        }

        public static OrderSummaryModelBuilder Create() => new();

        public OrderSummaryModelBuilder WithOrderId(string id)
        {
            orderId = id;
            return this;
        }

        public OrderSummaryModelBuilder WithOrganisationId(Guid id)
        {
            organisationId = id;
            return this;
        }

        public OrderSummaryModelBuilder WithSections(IEnumerable<SectionModel> orderSections)
        {
            sections = orderSections;
            return this;
        }

        public OrderSummaryModelBuilder WithSectionStatus(string status)
        {
            sectionStatus = status;
            return this;
        }

        public OrderSummaryModel Build()
        {
            return new()
            {
                OrderId = orderId,
                Description = description,
                OrganisationId = organisationId,
                Sections = sections,
                SectionStatus = sectionStatus,
                Status = status,
            };
        }
    }
}
