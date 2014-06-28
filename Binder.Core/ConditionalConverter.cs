using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;
using Expression = System.Linq.Expressions.Expression;

namespace Binder.Core
{
    public class ConditionalConverter : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string conditionFormat = parameter as string;
            if (string.IsNullOrWhiteSpace(conditionFormat))
                return DependencyProperty.UnsetValue;
            var rv = MethodManager.RunMethod(conditionFormat, values);

            if (targetType != null)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(targetType);
                if (rv != null && converter.CanConvertFrom(rv.GetType()))
                {
                    rv = converter.ConvertFrom(rv);
                }
            }

            return rv;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException(string.Format("{0} may only be used in one way bindings", GetType().FullName));
        }
    }
}
