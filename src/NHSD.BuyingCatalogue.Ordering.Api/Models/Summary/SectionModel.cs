using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models.Summary
{
    public sealed class SectionModel
    {
        private SectionModel(string id, string status = "incomplete")
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }

        public string Id { get; }

        public string Status { get; private set; }

        public int? Count { get; private set; }

        internal static SectionModel Description => new("description", "complete");

        internal static SectionModel OrderingParty => new("ordering-party");

        internal static SectionModel Supplier => new("supplier");

        internal static SectionModel CommencementDate => new("commencement-date");

        internal static SectionModel AssociatedServices => new("associated-services");

        internal static SectionModel ServiceRecipients => new("service-recipients");

        internal static SectionModel CatalogueSolutions => new("catalogue-solutions");

        internal static SectionModel AdditionalServices => new("additional-services");

        internal static SectionModel FundingSource => new("funding-source");

        public SectionModel WithStatus(string status)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
            return this;
        }

        public SectionModel WithCount(int count)
        {
            Count = count;
            return this;
        }
    }
}
