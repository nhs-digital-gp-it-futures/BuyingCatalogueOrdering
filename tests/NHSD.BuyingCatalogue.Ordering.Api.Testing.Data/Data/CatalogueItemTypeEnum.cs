namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data
{
    public enum CatalogueItemType
    {
        Solution = 1,
        AdditionalService = 2,
        AssociatedService = 3,
        Invalid = 4
    }

    public static class CatalogueItemTypeExtensions
    {
        public static string ToDisplayName(this CatalogueItemType timeUnit)
        {
            switch (timeUnit)
            {
                case CatalogueItemType.Solution:
                    return "Catalogue Solution";
                case CatalogueItemType.AdditionalService:
                    return "Additional Service";
                case CatalogueItemType.AssociatedService:
                    return "Associated Service";
                default:
                    return null;

            }
        }
    }
}
