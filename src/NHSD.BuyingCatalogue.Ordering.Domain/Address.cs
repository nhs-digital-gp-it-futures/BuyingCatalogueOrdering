namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Address
    {
        public int Id { get; set; }

        public string Line1 { get; init; }

        public string Line2 { get; init; }

        public string Line3 { get; init; }

        public string Line4 { get; init; }

        public string Line5 { get; init; }

        public string Town { get; init; }

        public string County { get; init; }

        public string Postcode { get; init; }

        public string Country { get; init; }
    }
}
