using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Beatshake.Converter
{
    public class ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string prefix = string.Empty;
            if (parameter != null && !string.IsNullOrWhiteSpace(parameter.ToString()))
            {
                prefix = parameter.ToString();
            }
            if (value != null)
            {
                return prefix + " " + value.ToString();
            }
            else
            {
                return prefix;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
