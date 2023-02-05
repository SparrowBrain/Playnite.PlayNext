using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PlayNext.Models;

namespace PlayNext.Converters
{
    internal class ShowcaseTypeTCollapsedVisibilityConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ShowcaseType selectedType &&
                parameter is ShowcaseType showcaseType &&
                selectedType == showcaseType)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}