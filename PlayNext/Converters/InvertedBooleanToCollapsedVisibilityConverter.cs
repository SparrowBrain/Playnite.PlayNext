﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PlayNext.Converters
{
    internal class InvertedBooleanToCollapsedVisibilityConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isTrue && isTrue)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}