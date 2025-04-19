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
            string str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return 0;

            try
            {
                if (targetType == typeof(byte))
                    return byte.Parse(str, NumberStyles.HexNumber);
                if (targetType == typeof(ushort))
                    return ushort.Parse(str, NumberStyles.HexNumber);
            }
            catch
            {
                // Игнорируем ошибки — возвращаем 0
            }

            return 0;
        }
    }

}
