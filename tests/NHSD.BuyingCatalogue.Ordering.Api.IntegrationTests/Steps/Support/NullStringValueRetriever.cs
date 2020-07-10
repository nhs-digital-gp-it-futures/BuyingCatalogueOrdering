using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support
{
    public sealed class NullStringValueRetriever : IValueRetriever
    {
        private const string NullString = "NULL";

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
            => IsNullValueMatch(keyValuePair.Value);

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
            => null;

        private static bool IsNullValueMatch(string value)
            => string.Equals(value?.Trim(), NullString, StringComparison.OrdinalIgnoreCase);
    }
}
