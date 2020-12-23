namespace NHSD.BuyingCatalogue.Ordering.Application.Persistence
{
    public interface IOrderQuery
    {
        IOrderQuery WithOrderItems();

        IOrderQuery WithOrganisationDetails();

        IOrderQuery WithServiceInstanceItems();

        IOrderQuery WithServiceRecipients();

        IOrderQuery WithSupplierDetails();

        IOrderQuery WithoutTracking();
    }
}
