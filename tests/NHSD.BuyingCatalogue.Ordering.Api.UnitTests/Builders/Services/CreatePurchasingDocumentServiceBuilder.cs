using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class CreatePurchasingDocumentServiceBuilder
    {
        private ICsvStreamWriter<PatientNumbersPriceType> _patientNumbersCsvStreamWriter;

        private CreatePurchasingDocumentServiceBuilder()
        {
        }

        public static CreatePurchasingDocumentServiceBuilder Create() =>
            new CreatePurchasingDocumentServiceBuilder();

        public CreatePurchasingDocumentServiceBuilder WithPatientNumbersCsvWriter(
            ICsvStreamWriter<PatientNumbersPriceType> patientNumbersCsvStreamWriter)
        {
            _patientNumbersCsvStreamWriter = patientNumbersCsvStreamWriter;
            return this;
        }

        public CreatePurchasingDocumentService Build() =>
            new CreatePurchasingDocumentService(_patientNumbersCsvStreamWriter);
    }
}
