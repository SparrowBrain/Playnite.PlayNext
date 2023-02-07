using System;
using System.Globalization;
using System.Windows.Data;

namespace PlayNext.Converters
{
    internal class WidthToHeightMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double actualWidth && values[1] is double ratio)
            {
                return actualWidth / ratio;
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}