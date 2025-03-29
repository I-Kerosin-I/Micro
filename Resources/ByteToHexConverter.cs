using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Micro.Resources
{
    public class ByteToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte b)
                return b.ToString("X2"); // Формат HEX
            return "00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (byte.TryParse((string)value, NumberStyles.HexNumber, null, out byte byteValue)) return byteValue;
            return 0;
        }
    }

}
