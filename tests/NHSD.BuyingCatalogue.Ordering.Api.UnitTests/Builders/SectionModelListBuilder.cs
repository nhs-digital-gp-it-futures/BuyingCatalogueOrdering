using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class SectionModelListBuilder
    {
        private SectionModel _description;
        private SectionModel _orderingParty;
        private SectionModel _supplier;
        private SectionModel _commencementDate;
        private readonly SectionModel _associatedServices;
        private SectionModel _serviceRecipients;
        private readonly SectionModel _catalogueSolutions;
        private readonly SectionModel _additionalServices;
        private readonly SectionModel _fundingSource;

        private SectionModelListBuilder()
        {
            _description = SectionModel.Description;
            _orderingParty = SectionModel.OrderingParty;
            _supplier = SectionModel.Supplier;
            _commencementDate = SectionModel.CommencementDate;
            _associatedServices = SectionModel.AssociatedServices;
            _serviceRecipients = SectionModel.ServiceRecipients;
            _catalogueSolutions = SectionModel.CatalogueSolutions;
            _additionalServices = SectionModel.AdditionalServices;
            _fundingSource = SectionModel.FundingSource;
        }

        public static SectionModelListBuilder Create()
        {
            return new SectionModelListBuilder();
        }

        public SectionModelListBuilder WithDescription(SectionModel description)
        {
            _description = description;
            return this;
        }

        public SectionModelListBuilder WithOrderingParty(SectionModel orderingParty)
        {
            _orderingParty = orderingParty;
            return this;
        }

        public SectionModelListBuilder WithSupplier(SectionModel supplier)
        {
            _supplier = supplier;
            return this;
        }

        public SectionModelListBuilder WithCommencementDate(SectionModel commencementDate)
        {
            _commencementDate = commencementDate;
            return this;
        }

        public SectionModelListBuilder WithServiceRecipients(SectionModel serviceRecipients)
        {
            _serviceRecipients = serviceRecipients;
            return this;
        }

        public IEnumerable<SectionModel> Build()
        {
            yield return _description;
            yield return _orderingParty;
            yield return _supplier;
            yield return _commencementDate;
            yield return _associatedServices;
            yield return _serviceRecipients;
            yield return _catalogueSolutions;
            yield return _additionalServices;
            yield return _fundingSource;
        }
    }
}
