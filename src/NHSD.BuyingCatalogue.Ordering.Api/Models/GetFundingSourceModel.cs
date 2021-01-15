using System.Text.Json.Serialization;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class GetFundingSourceModel
    {
        [JsonPropertyName("onlyGMS")]
        public bool? OnlyGms { get; set; }
    }
}
