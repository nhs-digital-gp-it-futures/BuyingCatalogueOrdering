using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class ErrorMessageModel
    {
        public string Id { get; }

        public string Field { get; }

        public ErrorMessageModel(string id, string field = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

    }
}
