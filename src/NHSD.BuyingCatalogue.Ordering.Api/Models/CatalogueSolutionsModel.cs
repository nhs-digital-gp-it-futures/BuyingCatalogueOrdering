using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CatalogueSolutionsModel
    {
        public string OrderDescription { get; set; }

        public IEnumerable<CatalogueSolutionModel> CatalogueSolutions { get; set; }
    }
}
