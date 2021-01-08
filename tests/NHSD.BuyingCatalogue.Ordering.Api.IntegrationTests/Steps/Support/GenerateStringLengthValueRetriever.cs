using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support
{
    public sealed class GenerateStringLengthValueRetriever : IValueRetriever
    {
        private const string PatternMatchGroupKey = "StringLength";

        private static readonly Regex _substituteStringPattern =
            new(@$"#A string of length (?<{PatternMatchGroupKey}>\d+)#", RegexOptions.IgnoreCase);

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
            => propertyType == typeof(string) && _substituteStringPattern.IsMatch(keyValuePair.Value);

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
            => Parse(keyValuePair.Value);

        private static string Parse(string value)
            => _substituteStringPattern.Replace(value, OnMatch);

        private static string OnMatch(Match match)
        {
            string returnValue = match.Value;

            var matchedValue = match.Groups[PatternMatchGroupKey];
            if (Int32.TryParse(matchedValue.Value, NumberStyles.Integer, new NumberFormatInfo(), out var stringLength))
            {
                returnValue = new string('a', stringLength);
            }

            return returnValue;
        }
    }
}
