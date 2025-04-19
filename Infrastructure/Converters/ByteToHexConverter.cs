using System;
using System.Globalization;
using System.Windows.Data;

namespace Micro.Infrastructure.Converters
{
    public class ByteToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = parameter as string ?? "X2";
            if (value is byte b)
                return b.ToString(format);
            if (value is ushort us)
                return us.ToString(format);
            return "00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (byte.TryParse((string)value, NumberStyles.HexNumber, null, out byte byteValue)) return byteValue;
            return 0;
        }
    }

}
