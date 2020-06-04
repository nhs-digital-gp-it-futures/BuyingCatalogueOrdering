using System;
using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models.Summary
{
    public sealed class OrderSummaryModel
    {
        public string OrderId { get; set; }

        public Guid OrganisationId { get; set; }

        public string Description { get; set; }

        public IEnumerable<SectionModel> Sections { get; set; }
    }
}
