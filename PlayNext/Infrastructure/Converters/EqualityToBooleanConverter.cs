using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PlayNext.Infrastructure.Converters
{
    internal class EqualityToBooleanConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return parameter;
            }

            return Binding.DoNothing;
        }
    }
}