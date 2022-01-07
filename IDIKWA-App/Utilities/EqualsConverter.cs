using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class EqualsConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any())
            {
                object first = values.First();
                for (int i = 1; i < values.Count; ++i)
                    if (first.GetHashCode() != values[i].GetHashCode())
                        return false;
                return true;
            }
            else
                return false;
        }
    }
}