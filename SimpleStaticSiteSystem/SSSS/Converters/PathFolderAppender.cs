using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace SSSS.Converters
{
    internal class PathFolderAppender : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var mainPath = values[0] as string ?? "";
            var folder = values[1] as string ?? "";
            return Path.Combine(mainPath, folder);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var fullPath = value as string ?? "";
            if (!string.IsNullOrEmpty(fullPath))
            {
                var pathSeparatorIndex = fullPath.LastIndexOf(Path.PathSeparator);
                if (pathSeparatorIndex > -1)
                {
                    return new[]
                    {
                        fullPath[..pathSeparatorIndex],
                        fullPath[pathSeparatorIndex..]
                    };
                }
            }
            throw new ArgumentException($"Cannot convert {fullPath} back into path and folder.");
        }
    }
}
