using CsvHelper.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    internal sealed class PatientNumbersPriceTypeMap : ClassMap<PatientNumbersPriceType>
    {
        public PatientNumbersPriceTypeMap()
        {
            Map(x => x.CallOffAgreementId).Index(0).Name("Call Off Agreement ID");
            Map(x => x.CallOffOrderingPartyId).Index(1).Name("Call Off Ordering Party ID");
            Map(x => x.CallOffOrderingPartyName).Index(2).Name("Call Off Ordering Party Name");
            Map(x => x.CallOffCommencementDate).Index(3).Name("Call Off Commencement Date");
            Map(x => x.ServiceRecipientId).Index(4).Name("Service Recipient ID");
            Map(x => x.ServiceRecipientName).Index(5).Name("Service Recipient Name");
            Map(x => x.ServiceRecipientItemId).Index(6).Name("Service Recipient Item ID");
            Map(x => x.SupplierId).Index(7).Name("Supplier ID");
            Map(x => x.SupplierName).Index(8).Name("Supplier Name");
            Map(x => x.ProductId).Index(9).Name("Product ID");
            Map(x => x.ProductName).Index(10).Name("Product Name");
            Map(x => x.ProductType).Index(11).Name("Product Type");
            Map(x => x.QuantityOrdered).Index(12).Name("Quantity Ordered");
            Map(x => x.UnitOfOrder).Index(13).Name("Unit of Order");
            Map(x => x.Price).Index(14).Name("Price");
            Map(x => x.FundingType).Index(15).Name("Funding Type");
            Map(x => x.M1Planned).Index(16).Name("M1 planned (Delivery Date)");
            Map(x => x.ActualM1Date).Index(17).Name("Actual M1 date");
            Map(x => x.BuyerVerificationDate).Index(18).Name("Buyer verification date (M2)");
            Map(x => x.CeaseDate).Index(19).Name("Cease Date");
        }
    }
}
