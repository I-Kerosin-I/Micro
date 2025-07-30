using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Micro.Infrastructure.Converters
{
    public class RgkBinaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var binvalue = System.Convert.ToString(System.Convert.ToUInt32(value.ToString(), 16), 2).PadLeft(16, '0');


            return $"{binvalue.Substring(0,5)} {binvalue.Substring(5, 3)} {binvalue.Substring(8, 2)} {binvalue.Substring(10, 3)} {binvalue.Substring(13, 3)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Обратное преобразование не нужно
        }
    }
}
