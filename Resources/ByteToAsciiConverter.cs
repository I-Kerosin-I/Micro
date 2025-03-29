using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace Micro.Resources
{


public class ByteToAsciiConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
            //if (value is string hexByte && byte.TryParse(hexByte, NumberStyles.HexNumber, null, out byte byteValue))
            //{
            //    char asciiChar = byteValue >= 32 && byteValue <= 126 ? (char)byteValue : '.'; // Только печатаемые символы, иначе '.'
            //    return asciiChar.ToString();
            //}
            //char asciiChar = (byte)value >= 32 && (byte)value <= 126 ? (char)value : '.'; // Только печатаемые символы, иначе '.'
            //return asciiChar.ToString();

            return (char)((byte)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string charStr && charStr.Length == 1)
            {
                
                return (byte)charStr[0]; // Преобразование обратно в HEX
            }
            return "00";
    }
}

}