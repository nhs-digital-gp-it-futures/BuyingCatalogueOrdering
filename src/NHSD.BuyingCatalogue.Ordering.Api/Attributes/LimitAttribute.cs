using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NHSD.BuyingCatalogue.Ordering.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public sealed class LimitAttribute : ValidationAttribute
    {
        public object Value { get; private set; }

        public Type OperandType { get; private set; }

        public LimitType RangeType { get; private set; }

        private Func<object, object> Conversion { get; set; }

        public LimitAttribute(int value, LimitType range)
            : this()
        {
            this.Value = value;
            this.RangeType = range;
            this.OperandType = typeof(int);
        }

        public LimitAttribute(double value, LimitType range)
            : this()
        {
            this.Value = value;
            this.RangeType = range;
            this.OperandType = typeof(double);
        }

        public LimitAttribute(Type type, string value, LimitType range)
            : this()
        {
            this.OperandType = type;
            this.Value = value;
            this.RangeType = range;
        }

        private LimitAttribute()
            : base(() => "Value was outside the defined limit")
        {
        }

        private void Initialize(IComparable value, Func<object, object> conversion)
        {
            this.Value = value;
            this.Conversion = conversion;
        }

        public override bool IsValid(object value)
        {
            // Validate our properties and create the conversion function
            this.SetupConversion();

            // Automatically pass if value is null or empty. RequiredAttribute should be used to assert a value is not empty.
            if (value == null)
            {
                return true;
            }

            if (value is string s && String.IsNullOrEmpty(s))
            {
                return true;
            }

            object convertedValue = null;

            try
            {
                convertedValue = this.Conversion(value);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

            IComparable val = (IComparable)this.Value;
            if (RangeType == LimitType.Minimum)
            {
                return val.CompareTo(convertedValue) <= 0;
            }
            else
            {
                return val.CompareTo(convertedValue) >= 0;
            }
        }

        /// <summary>
        /// Override of <see cref="ValidationAttribute.FormatErrorMessage"/>
        /// </summary>
        /// <remarks>This override exists to provide a formatted message describing the minimum and maximum values</remarks>
        /// <param name="name">The user-visible name to include in the formatted message.</param>
        /// <returns>A localized string describing the minimum and maximum values</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public override string FormatErrorMessage(string name)
        {
            this.SetupConversion();

            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, this.Value);
        }

        /// <summary>
        /// Validates the properties of this attribute and sets up the conversion function.
        /// This method throws exceptions if the attribute is not configured properly.
        /// If it has once determined it is properly configured, it is a NOP.
        /// </summary>
        private void SetupConversion()
        {
            if (this.Conversion == null)
            {
                if (this.Value == null)
                {
                    throw new InvalidOperationException("Must set a value to be checked");
                }

                // Careful here -- OperandType could be int or double if they used the long form of the ctor.
                // But the min and max would still be strings.  Do use the type of the min/max operands to condition
                // the following code.
                Type operandType = this.Value.GetType();

                if (operandType == typeof(int))
                {
                    this.Initialize((int)this.Value, v => Convert.ToInt32(v, CultureInfo.InvariantCulture));
                }
                else if (operandType == typeof(double))
                {
                    this.Initialize((double)this.Value, v => Convert.ToDouble(v, CultureInfo.InvariantCulture));
                }
                else
                {
                    Type type = this.OperandType;
                    if (type == null)
                    {
                        throw new InvalidOperationException("Limit attribute must set Operand type for string inputs");
                    }
                    Type comparableType = typeof(IComparable);
                    if (!comparableType.IsAssignableFrom(type))
                    {
                        throw new InvalidOperationException(
                            $"Type {comparableType.FullName} is not assignable from {type.FullName}");
                    }

                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    IComparable val = (IComparable)converter.ConvertFromString((string)this.Value);

                    this.Initialize(val, 
                        value => (value != null && value.GetType() == type) 
                            ? value 
                            : converter.ConvertFrom(value));
                }
            }
        }
    }
}
