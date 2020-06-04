using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class ErrorModel
    {
        public string Id { get; }

        public string Field { get; }

        public ErrorModel(string id, string field = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }
    }
}
