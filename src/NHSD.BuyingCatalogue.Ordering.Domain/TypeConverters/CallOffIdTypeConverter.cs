using System;
using System.ComponentModel;
using System.Globalization;

namespace NHSD.BuyingCatalogue.Ordering.Domain.TypeConverters
{
    public sealed class CallOffIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            CallOffId.Parse((string)value).Id;
    }
}
