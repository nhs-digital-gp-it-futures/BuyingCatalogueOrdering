using System.Diagnostics.CodeAnalysis;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public abstract class CreateOrderItemRequest : UpdateOrderItemRequest
    {
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "model is validated by base constructor")]
        protected CreateOrderItemRequest(Order order, CreateOrderItemModel model, CatalogueItemType itemType)
            : base(order, model)
        {
            CatalogueItemId = model.CatalogueItemId;
            CatalogueItemType = itemType;
            CatalogueItemName = model.CatalogueItemName;
            ProvisioningType = OrderingEnums.ParseStrictIgnoreCase<ProvisioningType>(model.ProvisioningType);
            CataloguePriceType = OrderingEnums.ParseStrictIgnoreCase<CataloguePriceType>(model.Type);
            CataloguePriceUnitTierName = model.ItemUnit.Name;
            CataloguePriceUnitDescription = model.ItemUnit.Description;
            CurrencyCode = model.CurrencyCode;
        }

        public ServiceRecipient ServiceRecipient { get; protected set; }

        public string OdsCode { get; protected set; }

        public string CatalogueItemId { get; }

        public CatalogueItemType CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public string CatalogueSolutionId { get; protected set; }

        public ProvisioningType ProvisioningType { get; }

        public CataloguePriceType CataloguePriceType { get; }

        public string CataloguePriceUnitTierName { get; }

        public string CataloguePriceUnitDescription { get; }

        public TimeUnit? PriceTimeUnit { get; protected set; }

        public string CurrencyCode { get; }
    }
}
