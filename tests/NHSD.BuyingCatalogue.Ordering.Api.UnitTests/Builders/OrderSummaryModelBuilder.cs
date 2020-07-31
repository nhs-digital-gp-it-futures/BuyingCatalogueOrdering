using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrderSummaryModelBuilder
    {
        private string _orderId;
        private readonly string _description;
        private Guid _organisationId;
        private IEnumerable<SectionModel> _sections;
        private string _sectionStatus;
        private readonly string _status;

        private OrderSummaryModelBuilder()
        {
            _orderId = "C000014-01";
            _description = "Some Description";
            _organisationId = Guid.NewGuid();
            _sections = SectionModelListBuilder.Create().Build();
            _sectionStatus = "incomplete";
            _status = OrderStatus.Unsubmitted.ToString();
        }

        public static OrderSummaryModelBuilder Create()
        {
            return new OrderSummaryModelBuilder();
        }

        public OrderSummaryModelBuilder WithOrderId(string orderId)
        {
            _orderId = orderId;
            return this;
        }

        public OrderSummaryModelBuilder WithOrganisationId(Guid organisationId)
        {
            _organisationId = organisationId;
            return this;
        }

        public OrderSummaryModelBuilder WithSections(IEnumerable<SectionModel> sections)
        {
            _sections = sections;
            return this;
        }

        public OrderSummaryModelBuilder WithSectionStatus(string sectionStatus)
        {
            _sectionStatus = sectionStatus;
            return this;
        }

        public OrderSummaryModel Build()
        {
            return new OrderSummaryModel
            {
                OrderId = _orderId,
                Description = _description,
                OrganisationId = _organisationId,
                Sections = _sections,
                SectionStatus = _sectionStatus,
                Status = _status
            };
        }
    }
}
