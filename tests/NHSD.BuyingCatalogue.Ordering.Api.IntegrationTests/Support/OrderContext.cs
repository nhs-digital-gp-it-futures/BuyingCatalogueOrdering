namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class OrderContext
    {
        public OrderContext()
        {
            AddressReferenceList = new AddressReferenceList();
            ContactReferenceList = new ContactReferenceList();
            OrderReferenceList = new OrderReferenceList();
            OrderItemReferenceList = new OrderItemReferenceList();
            ServiceRecipientReferenceList = new ServiceRecipientReferenceList();
        }

        public AddressReferenceList AddressReferenceList { get; }

        public ContactReferenceList ContactReferenceList { get; }

        public OrderReferenceList OrderReferenceList { get; }

        public OrderItemReferenceList OrderItemReferenceList { get; }

        public ServiceRecipientReferenceList ServiceRecipientReferenceList { get; }
    }
}
