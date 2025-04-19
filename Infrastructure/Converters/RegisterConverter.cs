using System;
using System.Globalization;
using System.Windows.Data;

namespace Micro.Infrastructure.Converters
{
    public class RegisterLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{parameter}{value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Обратное преобразование не нужно
        }
    }
}
