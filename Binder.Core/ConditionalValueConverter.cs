using System;
using System.Globalization;
using System.Windows.Data;

namespace Binder.Core
{
    public class ConditionalValueConverter : ConditionalConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertValues(new[] {value}, targetType, parameter as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException(string.Format("{0} may only be used in one way bindings", GetType().FullName));
        }
    }
}