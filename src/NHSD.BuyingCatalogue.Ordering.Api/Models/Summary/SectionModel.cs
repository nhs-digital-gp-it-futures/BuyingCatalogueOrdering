using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models.Summary
{
    public sealed class SectionModel
    {
        internal static SectionModel Description => new SectionModel("description", "complete");
        internal static SectionModel OrderingParty => new SectionModel("ordering-party");
        internal static SectionModel Supplier => new SectionModel("supplier");
        internal static SectionModel CommencementDate => new SectionModel("commencement-date");
        internal static SectionModel AssociatedServices => new SectionModel("associated-services");
        internal static SectionModel ServiceRecipients => new SectionModel("service-recipients");
        internal static SectionModel CatalogueSolutions => new SectionModel("catalogue-solutions");
        internal static SectionModel AdditionalServices => new SectionModel("additional-services");
        internal static SectionModel FundingSource => new SectionModel("funding-source");

        public string Id { get; }

        public string Status { get; private set; }

        private SectionModel(string id, string status = "incomplete")
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }

        public SectionModel WithStatus(string status)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
            return this;
        }
    }
}
