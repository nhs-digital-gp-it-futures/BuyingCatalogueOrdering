namespace NHSD.BuyingCatalogue.Ordering.Api.Models.Errors
{
    internal static class ErrorMessages
    {
        public static ErrorModel InvalidOrderStatus() => new("InvalidStatus", "Status");
    }
}
