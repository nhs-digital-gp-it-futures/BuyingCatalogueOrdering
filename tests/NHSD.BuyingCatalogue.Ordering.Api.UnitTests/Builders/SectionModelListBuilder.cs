using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class SectionModelListBuilder
    {
        private SectionModel description;
        private SectionModel orderingParty;
        private SectionModel supplier;
        private SectionModel commencementDate;
        private SectionModel associatedServices;
        private SectionModel serviceRecipients;
        private SectionModel catalogueSolutions;
        private SectionModel additionalServices;
        private SectionModel fundingSource;

        private SectionModelListBuilder()
        {
            description = SectionModel.Description;
            orderingParty = SectionModel.OrderingParty;
            supplier = SectionModel.Supplier;
            commencementDate = SectionModel.CommencementDate;
            associatedServices = SectionModel.AssociatedServices.WithCount(0);
            serviceRecipients = SectionModel.ServiceRecipients.WithCount(0);
            catalogueSolutions = SectionModel.CatalogueSolutions.WithCount(0);
            additionalServices = SectionModel.AdditionalServices.WithCount(0);
            fundingSource = SectionModel.FundingSource;
        }

        public static SectionModelListBuilder Create() => new();

        public SectionModelListBuilder WithDescription(SectionModel descriptionSection)
        {
            description = descriptionSection;
            return this;
        }

        public SectionModelListBuilder WithOrderingParty(SectionModel orderingPartySection)
        {
            orderingParty = orderingPartySection;
            return this;
        }

        public SectionModelListBuilder WithSupplier(SectionModel supplierSection)
        {
            supplier = supplierSection;
            return this;
        }

        public SectionModelListBuilder WithCommencementDate(SectionModel commencementDateSection)
        {
            commencementDate = commencementDateSection;
            return this;
        }

        public SectionModelListBuilder WithServiceRecipients(SectionModel serviceRecipientsSection)
        {
            serviceRecipients = serviceRecipientsSection;
            return this;
        }

        public SectionModelListBuilder WithAdditionalServices(SectionModel additionalServicesSection)
        {
            additionalServices = additionalServicesSection;
            return this;
        }

        public SectionModelListBuilder WithCatalogueSolutions(SectionModel catalogueSolutionsSection)
        {
            catalogueSolutions = catalogueSolutionsSection;
            return this;
        }

        public SectionModelListBuilder WithAssociatedServices(SectionModel associatedServicesSection)
        {
            associatedServices = associatedServicesSection;
            return this;
        }

        public SectionModelListBuilder WithFundingSource(SectionModel fundingSourceSection)
        {
            fundingSource = fundingSourceSection;
            return this;
        }

        public IEnumerable<SectionModel> Build()
        {
            yield return description;
            yield return orderingParty;
            yield return supplier;
            yield return commencementDate;
            yield return associatedServices;
            yield return serviceRecipients;
            yield return catalogueSolutions;
            yield return additionalServices;
            yield return fundingSource;
        }
    }
}
