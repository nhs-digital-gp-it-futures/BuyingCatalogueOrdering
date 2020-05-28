using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class SectionModelListBuilder
    {
        private readonly SectionModel _description;
        private readonly SectionModel _orderingParty;
        private SectionModel _supplier;
        private readonly SectionModel _commencementDate;
        private readonly SectionModel _associatedServices;
        private readonly SectionModel _serviceRecipients;
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

        public SectionModelListBuilder WithSupplier(SectionModel supplier)
        {
            _supplier = supplier;
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
