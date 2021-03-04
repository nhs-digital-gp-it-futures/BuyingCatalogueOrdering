using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests
{
    public static class BackingField
    {
        public static void AddListItem<T>(object obj, string propertyName, T value)
        {
            var list = GetListBackingField<T>(obj, propertyName);
            list.Add(value);
        }

        public static void AddListItems<T>(object obj, string propertyName, IEnumerable<T> values)
        {
            var list = GetListBackingField<T>(obj, propertyName);
            list.AddRange(values);
        }

        public static void SetValue(object obj, string propertyName, object value)
        {
            var field = GetBackingField(obj, propertyName);
            field.SetValue(obj, value);
        }

        private static List<T> GetListBackingField<T>(object obj, string propertyName) =>
            GetBackingField<List<T>>(obj, propertyName);

        private static FieldInfo GetBackingField(object obj, string propertyName)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is required.", nameof(propertyName));

            var camelCaseFieldName = char.ToLowerInvariant(propertyName[0]) + propertyName[1..];

            var fieldInfo = obj.GetType().GetField(camelCaseFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo is null)
                throw new InvalidOperationException($"A backing field for {propertyName} does not exist.");

            return fieldInfo;
        }

        private static T GetBackingField<T>(object obj, string propertyName)
        {
            var fieldInfo = GetBackingField(obj, propertyName);

            if (!(fieldInfo.GetValue(obj) is T backingField))
                throw new InvalidOperationException($"The backing field for {propertyName} is not of type {typeof(T).Name}.");

            return backingField;
        }
    }
}
