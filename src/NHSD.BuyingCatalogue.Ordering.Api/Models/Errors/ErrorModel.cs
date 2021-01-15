using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models.Errors
{
    public sealed class ErrorModel
    {
        public ErrorModel(string id, string field = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

        public string Id { get; }

        public string Field { get; }
    }
}
