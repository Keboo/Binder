using System;
using System.Globalization;
using System.Windows.Data;

namespace Binder.Core
{
    public class ConstructorInvocationValueConverter : ConstructorInvocationConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return CreateObject(new[] { value }, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException(string.Format("{0} may only be used in one way bindings", GetType().FullName));
        }
    }
}