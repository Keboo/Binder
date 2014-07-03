using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Binder.Core
{
    public class MethodInvocationValueConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var methodName = parameter as string;
            if (string.IsNullOrWhiteSpace(methodName) || value == null)
                return DependencyProperty.UnsetValue;
            MethodInfo methodInfo = value.GetType().GetMethod(methodName, new Type[0]);
            if (methodInfo == null)
            {
                Debug.WriteLine("{0}: Could not find parameter-less method '{1}' on object '{2}'", GetType().FullName,
                    methodName, value.GetType().FullName);
                return DependencyProperty.UnsetValue;
            }
            try
            {
                return methodInfo.Invoke(value, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} - EXCEPTION: {1}", GetType().FullName, ex);
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException(string.Format("{0} may only be used in one way bindings", GetType().FullName));
        }
    }
}