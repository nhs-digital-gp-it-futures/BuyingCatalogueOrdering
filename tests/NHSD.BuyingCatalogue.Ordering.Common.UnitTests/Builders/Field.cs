using System.Reflection;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders
{
    internal static class Field
    {
        internal static void Set(object obj, string fieldName, object value)
        {
            var camelCaseFieldName = char.ToLowerInvariant(fieldName[0]) + fieldName[1..];

            var fieldInfo = obj.GetType().GetField(camelCaseFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo?.SetValue(obj, value);
        }
    }
}
