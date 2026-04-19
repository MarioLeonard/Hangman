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
                // If it is already a pack URI, return as-is
                if (relativePath.StartsWith("pack://"))
                {
                    return relativePath;
                }

                // Build the absolute path
                string absolutePath;
                if (Path.IsPathRooted(relativePath))
                {
                    absolutePath = relativePath;
                }
                else
                {
                    absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                }

                // Check if the file exists; if not, use default image
                if (File.Exists(absolutePath))
                {
                    return absolutePath;
                }
                else
                {
                    // Return default image if the file doesn't exist
                    string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "default.jpg");
                    return File.Exists(defaultImagePath) ? defaultImagePath : null;
                }
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
