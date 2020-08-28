using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class CreatePurchasingDocumentServiceBuilder
    {
        private IAttachmentCsvWriter<PatientNumbersPriceType> _patientNumbersCsvWriter;

        private CreatePurchasingDocumentServiceBuilder()
        {
        }

        public static CreatePurchasingDocumentServiceBuilder Create() =>
            new CreatePurchasingDocumentServiceBuilder();

        public CreatePurchasingDocumentServiceBuilder WithPatientNumbersCsvWriter(
            IAttachmentCsvWriter<PatientNumbersPriceType> patientNumbersCsvWriter)
        {
            _patientNumbersCsvWriter = patientNumbersCsvWriter;
            return this;
        }

        public CreatePurchasingDocumentService Build() =>
            new CreatePurchasingDocumentService(_patientNumbersCsvWriter);
    }
}
