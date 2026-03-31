using System;
using System.Globalization;
using System.Windows.Data;

namespace Hangman_Game.Converters;

public class CategoryToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string selectedCategory && parameter is string categoryParameter)
        {
            return string.Equals(selectedCategory, categoryParameter, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
