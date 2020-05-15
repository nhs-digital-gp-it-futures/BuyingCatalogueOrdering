using System;
using System.Collections.Generic;
using System.Globalization;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support
{
    public sealed class StringValueRetriever : IValueRetriever
    {
        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(string);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            const string nullString = "NULL";
            const string lengthString = "A string with the length of ";

            if (keyValuePair.Value == nullString)
            {
                return null;
            }

            if (keyValuePair.Value.StartsWith(lengthString, StringComparison.InvariantCulture))
            {
                var value = keyValuePair.Value.Replace(lengthString, string.Empty, StringComparison.InvariantCulture);
                return new string('a', Int32.Parse(value, NumberStyles.Integer, new NumberFormatInfo()));
            }

            return keyValuePair.Value.Trim('"');
        }
    }
}
