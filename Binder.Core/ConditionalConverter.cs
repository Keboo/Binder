using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Binder.Core
{
    public abstract class ConditionalConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        protected object ConvertValues(object[] values, Type targetType, string conditionFormat)
        {
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
    }
}