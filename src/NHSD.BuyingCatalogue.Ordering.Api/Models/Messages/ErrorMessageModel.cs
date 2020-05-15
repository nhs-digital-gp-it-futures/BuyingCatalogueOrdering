using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models.Messages
{
    public sealed class ErrorMessageModel
    {
        public string Id { get; }

        public string Field { get; }

        public ErrorMessageModel(string id, string field)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }
    }
}
