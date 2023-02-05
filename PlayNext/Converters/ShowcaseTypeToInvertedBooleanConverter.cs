using System;
using System.Globalization;
using System.Windows.Data;
using PlayNext.Models;

namespace PlayNext.Converters
{
    internal class ShowcaseTypeToInvertedBooleanConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ShowcaseType selectedType &&
                parameter is ShowcaseType showcaseType)
            {
                return selectedType != showcaseType;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}