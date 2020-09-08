using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class CreatePurchasingDocumentServiceBuilder
    {
        private ICsvStreamWriter<PatientNumbersPriceType> _patientNumbersCsvWriter;
        private ICsvStreamWriter<PriceType> _priceTypeCsvWriter;

        private CreatePurchasingDocumentServiceBuilder()
        {
        }

        public static CreatePurchasingDocumentServiceBuilder Create() =>
            new CreatePurchasingDocumentServiceBuilder();

        public CreatePurchasingDocumentServiceBuilder WithPatientNumbersCsvWriter(
            ICsvStreamWriter<PatientNumbersPriceType> patientNumbersCsvStreamWriter)
        {
            _patientNumbersCsvWriter = patientNumbersCsvStreamWriter;
            return this;
        }

        public CreatePurchasingDocumentServiceBuilder WithPriceTypeCsvWriter(
            ICsvStreamWriter<PriceType> priceType)
        {
            _priceTypeCsvWriter = priceType;
            return this;
        }

        public CreatePurchasingDocumentService Build() =>
            new CreatePurchasingDocumentService(_patientNumbersCsvWriter, _priceTypeCsvWriter);
    }
}
