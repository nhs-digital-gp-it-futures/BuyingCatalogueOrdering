using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemAssociatedServiceModel : CreateOrderItemModel
    {
        protected override CatalogueItemType GetItemType() => Domain.CatalogueItemType.AssociatedService;

        protected override string GetOdsCode(Order order) => order.OrganisationOdsCode;

        protected override TimeUnit? TimeUnitModelToTimeUnit() => null;
    }
}
