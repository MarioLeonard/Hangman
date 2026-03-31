using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Hangman_Game.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string relativePath && !string.IsNullOrEmpty(relativePath))
            {
                // If it is already an absolute path or pack URI, return as-is
                if (Path.IsPathRooted(relativePath) || relativePath.StartsWith("pack://"))
                {
                    return relativePath;
                }

                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
