using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    internal class ResxConverter : IValueConverter
    {
        private Resx resourceLoader;

        public ResxConverter()
        {
            resourceLoader = new Resx();
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string key)
            {
                resourceLoader.Key = key;
                return resourceLoader.ProvideValue(null);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}