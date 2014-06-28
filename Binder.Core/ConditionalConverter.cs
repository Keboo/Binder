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
        private static readonly IDictionary<MethodSignature, Func<object[], object>> _methods = new Dictionary<MethodSignature, Func<object[], object>>();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string conditionFormat = parameter as string;
            if (string.IsNullOrWhiteSpace(conditionFormat))
                return DependencyProperty.UnsetValue;
            var rv = ConvertValues(conditionFormat, values);

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

        private static object ConvertValues(string conditionFormat, object[] parameters)
        {
            lock (_methods)
            {
                Type[] parameterTypes = parameters.Select(x => x != null ? x.GetType() : typeof (object)).ToArray();
                var signature = new MethodSignature(conditionFormat, parameterTypes);
                Func<object[], object> method;
                if (_methods.TryGetValue(signature, out method) == false)
                {
                    _methods[signature] = method = GenerateMethod(conditionFormat, parameterTypes);
                }
                return method(parameters);
            }
        }

        private static Func<object[], object> GenerateMethod(string conditionFormat, Type[] parameterTypes)
        {
            string formattedExpression = string.Format(conditionFormat,
                Enumerable.Range(0, parameterTypes.Length).Select(x => (object)string.Format("param{0}", x)).ToArray());
            var funcParameters =
                Enumerable.Range(0, parameterTypes.Length)
                .Select(x => Expression.Parameter(parameterTypes[x], string.Format("param{0}", x))).ToArray();
            
            LambdaExpression expression = DynamicExpression.ParseLambda(funcParameters, typeof(object), formattedExpression);
            ParameterExpression initalParameter = Expression.Parameter(typeof(object[]));
            var invokeParameters = Enumerable.Range(0, parameterTypes.Length)
                    .Select(x => Expression.ConvertChecked(Expression.ArrayIndex(initalParameter, Expression.Constant(x)),
                                parameterTypes[x]));
            var invokeExpression = Expression.Invoke(expression, invokeParameters);
            return Expression.Lambda<Func<object[], object>>(invokeExpression, initalParameter).Compile();
        }
    }
}
