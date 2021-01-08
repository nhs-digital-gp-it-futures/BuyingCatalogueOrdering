using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class CreatePurchasingDocumentServiceBuilder
    {
        private ICsvStreamWriter<OdooPatientNumbersOrderItem> patientNumbersCsvWriter;
        private ICsvStreamWriter<OdooOrderItem> priceTypeCsvWriter;

        private CreatePurchasingDocumentServiceBuilder()
        {
        }

        public static CreatePurchasingDocumentServiceBuilder Create() => new();

        public CreatePurchasingDocumentServiceBuilder WithPatientNumbersCsvWriter(
            ICsvStreamWriter<OdooPatientNumbersOrderItem> patientNumbersCsvStreamWriter)
        {
            patientNumbersCsvWriter = patientNumbersCsvStreamWriter;
            return this;
        }

        public CreatePurchasingDocumentServiceBuilder WithPriceTypeCsvWriter(
            ICsvStreamWriter<OdooOrderItem> priceType)
        {
            priceTypeCsvWriter = priceType;
            return this;
        }

        public CreatePurchasingDocumentService Build() => new(patientNumbersCsvWriter, priceTypeCsvWriter);
    }
}
