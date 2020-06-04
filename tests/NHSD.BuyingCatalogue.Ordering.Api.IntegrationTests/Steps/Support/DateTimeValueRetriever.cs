using System;
using System.Collections.Generic;
using System.Globalization;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support
{
    public sealed class DateTimeValueRetriever : IValueRetriever
    {
        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            string propertyName = keyValuePair.Key;
            return targetType?.GetProperty(propertyName)?.PropertyType == typeof(DateTime);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return DateTime.ParseExact(keyValuePair.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
