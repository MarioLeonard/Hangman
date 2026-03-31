using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hangman_Game.Converters;

public class NullableBooleanToVisibilityConverter : IValueConverter
{
    public bool TargetValue { get; set; } = true;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue == TargetValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
