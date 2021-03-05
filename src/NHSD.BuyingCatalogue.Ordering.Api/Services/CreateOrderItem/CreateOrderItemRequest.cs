using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    internal sealed class CreateOrderItemRequest
    {
        public CreateOrderItemRequest(CatalogueItemId catalogueItemId, CreateOrderItemModel model)
        {
            CatalogueItemId = catalogueItemId;
            CatalogueItemType = OrderingEnums.ParseStrictIgnoreCase<CatalogueItemType>(model.CatalogueItemType);
            CatalogueItemName = model.CatalogueItemName;
            ProvisioningType = OrderingEnums.ParseStrictIgnoreCase<ProvisioningType>(model.ProvisioningType);
            CataloguePriceType = OrderingEnums.ParseStrictIgnoreCase<CataloguePriceType>(model.Type);
            CataloguePriceUnitName = model.ItemUnit.Name;
            CataloguePriceUnitDescription = model.ItemUnit.Description;
            CatalogueSolutionId = model.CatalogueSolutionId;
            CurrencyCode = model.CurrencyCode;
            PriceTimeUnit = model.TimeUnit?.ToTimeUnit();
        }

        public Order Order { get; set; }

        public ServiceRecipient ServiceRecipient { get; set; }

        public CatalogueItemId CatalogueItemId { get; }

        public string CatalogueItemName { get; }

        public CatalogueItemType CatalogueItemType { get; }

        public CataloguePriceType CataloguePriceType { get; }

        public string CataloguePriceUnitDescription { get; }

        public string CataloguePriceUnitName { get; }

        public string CatalogueSolutionId { get; }

        public string CurrencyCode { get; }

        public TimeUnit? PriceTimeUnit { get; }

        public ProvisioningType ProvisioningType { get; }
    }
}
