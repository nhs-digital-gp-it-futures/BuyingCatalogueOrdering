using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support
{
    [Binding]
    public sealed class StepArgumentTransformation
    {
        [StepArgumentTransformation]
        internal static string ParseStringToNull(string nullString) => nullString is "NULL" ? null : nullString;
    }
}
