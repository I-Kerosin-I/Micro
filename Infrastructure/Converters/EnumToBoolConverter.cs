using System;
using System.Globalization;
using System.Windows.Data;

namespace Micro.Infrastructure.Converters
{
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) // Выдаёт true, если value (значение enum) и parameter равны
        {
            if (value == null || parameter == null)
                return false;
            return value.ToString().Equals(parameter.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) // Возвращает значение enum-а targetType по строке parameter
        {
            if (parameter == null)
                return null;

            return Enum.Parse(targetType, parameter.ToString());
        }
    }
}
