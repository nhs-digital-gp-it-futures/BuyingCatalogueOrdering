namespace NHSD.BuyingCatalogue.Ordering.Domain.Orders
{
    public static class OrderErrors
    {
        public static ErrorDetails OrderDescriptionRequired()
        {
            return new ErrorDetails("OrderDescriptionRequired", nameof(Order.Description));
        }

        public static ErrorDetails OrderDescriptionTooLong()
        {
            return new ErrorDetails("OrderDescriptionTooLong", nameof(Order.Description));
        }

        public static ErrorDetails OrderOrganisationIdRequired()
        {
            return new ErrorDetails("OrganisationIdRequired", nameof(Order.OrganisationId));
        }
    }
}
