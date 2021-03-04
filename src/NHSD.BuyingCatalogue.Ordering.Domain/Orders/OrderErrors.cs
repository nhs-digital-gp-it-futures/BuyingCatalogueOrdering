namespace NHSD.BuyingCatalogue.Ordering.Domain.Orders
{
    internal static class OrderErrors
    {
        public static ErrorDetails OrderNotComplete()
        {
            return new("OrderNotComplete");
        }
    }
}
