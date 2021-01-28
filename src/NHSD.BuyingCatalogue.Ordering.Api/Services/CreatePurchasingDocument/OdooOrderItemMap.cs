using CsvHelper.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    internal sealed class OdooOrderItemMap : ClassMap<OdooOrderItem>
    {
        public OdooOrderItemMap()
        {
            Map(i => i.CallOffAgreementId).Index(0).Name("Call Off Agreement ID");
            Map(i => i.CallOffOrderingPartyId).Index(1).Name("Call Off Ordering Party ID");
            Map(i => i.CallOffOrderingPartyName).Index(2).Name("Call Off Ordering Party Name");
            Map(i => i.CallOffCommencementDate).Index(3).Name("Call Off Commencement Date");
            Map(i => i.ServiceRecipientId).Index(4).Name("Service Recipient ID");
            Map(i => i.ServiceRecipientName).Index(5).Name("Service Recipient Name");
            Map(i => i.ServiceRecipientItemId).Index(6).Name("Service Recipient Item ID");
            Map(i => i.SupplierId).Index(7).Name("Supplier ID");
            Map(i => i.SupplierName).Index(8).Name("Supplier Name");
            Map(i => i.ProductId).Index(9).Name("Product ID");
            Map(i => i.ProductName).Index(10).Name("Product Name");
            Map(i => i.ProductType).Index(11).Name("Product Type");
            Map(i => i.QuantityOrdered).Index(12).Name("Quantity Ordered");
            Map(i => i.UnitOfOrder).Index(13).Name("Unit of Order");
            Map(i => i.UnitTime).Index(14).Name("Unit Time");
            Map(i => i.EstimationPeriod).Index(15).Name("Estimation Period");
            Map(i => i.Price).Index(16).Name("Price");
            Map(i => i.OrderType).Index(17).Name("Order Type");
            Map(i => i.FundingType).Index(18).Name("Funding Type");
            Map(i => i.M1Planned).Index(19).Name("M1 planned (Delivery Date)");
            Map(i => i.ActualM1Date).Index(20).Name("Actual M1 date");
            Map(i => i.BuyerVerificationDate).Index(21).Name("Buyer verification date (M2)");
            Map(i => i.CeaseDate).Index(22).Name("Cease Date");
        }
    }
}
