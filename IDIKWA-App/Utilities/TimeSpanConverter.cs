using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class TimeSpanConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan time)
            {
                var result = "";
                if (time.Hours != 0)
                    result += $"{time.Hours}h ";
                if (time.Minutes != 0)
                    result += $"{time.Minutes}min ";
                if (time.Seconds != 0 || (time.Hours == 0 && time.Minutes == 0))
                    result += $"{time.Seconds}s";
                return result;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}