using System;
using System.Globalization;
using System.Windows.Data;
using PlayNext.Model.Data;

namespace PlayNext.Infrastructure.Converters
{
    internal class ShowcaseTypeToBooleanConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ShowcaseType selectedType &&
                parameter is ShowcaseType showcaseType)
            {
                return selectedType == showcaseType;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
