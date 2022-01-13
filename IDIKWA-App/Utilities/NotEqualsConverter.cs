using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    internal class NotEqualsConverter : EqualsConverter
    {
        public override object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            return base.Convert(values, targetType, parameter, culture) is not true;
        }
    }
}