namespace NHSD.BuyingCatalogue.Ordering.Domain.Orders
{
    internal static class OrderErrors
    {
        public static ErrorDetails OrderDescriptionRequired()
        {
            return new("OrderDescriptionRequired", nameof(Order.Description));
        }

        public static ErrorDetails OrderDescriptionTooLong()
        {
            return new("OrderDescriptionTooLong", nameof(Order.Description));
        }

        public static ErrorDetails OrderOrganisationIdRequired()
        {
            return new("OrganisationIdRequired", nameof(Order.OrganisationId));
        }

        public static ErrorDetails OrderNotComplete()
        {
            return new("OrderNotComplete");
        }
    }
}
