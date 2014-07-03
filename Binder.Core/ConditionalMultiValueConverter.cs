using System;
using System.Globalization;
using System.Windows.Data;

namespace Binder.Core
{
    public class ConditionalMultiValueConverter : ConditionalConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertValues(values, targetType, parameter as string);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException(string.Format("{0} may only be used in one way bindings", GetType().FullName));
        }
    }
}
