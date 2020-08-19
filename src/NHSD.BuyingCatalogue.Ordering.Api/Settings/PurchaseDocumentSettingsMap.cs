using CsvHelper.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.Settings
{
    public sealed class PurchaseDocumentSettingsMap : ClassMap<PurchaseDocumentSettings>
    {
        public PurchaseDocumentSettingsMap()
        {
            Map(x => x.CallOffPartyId).Index(0).Name("Call off Party Id");
        }
    }
}
