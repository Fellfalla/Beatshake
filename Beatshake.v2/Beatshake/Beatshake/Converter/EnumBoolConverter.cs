using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.ViewModels;
using Xamarin.Forms;

namespace Beatshake.Converter
{
    public class EnumBoolConverter : IValueConverter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">The exptected enum value</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            //Type enumType = value.GetType();
            
            //var expectedEnumValue = Enum.Parse(enumType, parameter.ToString());
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                //var expectedEnumValue = Enum.Parse(targetType, parameter.ToString());
                return parameter;
            }
            else
            {
                
                return (DrumMode)parameter | DrumMode.None;
            }

        }
    }
}
