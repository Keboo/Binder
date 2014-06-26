using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Data;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;
using Expression = System.Linq.Expressions.Expression;


namespace Binder.Core
{
    public class ConditionalConverter : IMultiValueConverter
    {
        private static readonly IDictionary<string, Func<object[], object>> _methods = new Dictionary<string, Func<object[], object>>();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string conditionFormat = parameter as string;
            if (string.IsNullOrWhiteSpace(conditionFormat))
                return DependencyProperty.UnsetValue;
            return ConvertValues(conditionFormat, values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException(string.Format("{0} may only be used in one way bindings", GetType().FullName));
        }

        private static object ConvertValues(string conditionFormat, object[] parameters)
        {
            lock (_methods)
            {
                Func<object[], object> method;
                if (_methods.TryGetValue(conditionFormat, out method) == false)
                {
                    _methods[conditionFormat] = method = GenerateMethod(conditionFormat, parameters);
                }
                return method(parameters);
            }
        }

        private static Func<object[], object> GenerateMethod(string conditionFormat, object[] parameters)
        {
            string formattedExpression = string.Format(conditionFormat,
                Enumerable.Range(0, parameters.Length).Select(x => (object)string.Format("param{0}", x)).ToArray());
            var funcParameters =
                Enumerable.Range(0, parameters.Length)
                    .Select(x => Expression.Parameter(parameters[x].GetType(), string.Format("param{0}", x)))
                    .ToArray();

            LambdaExpression expression = DynamicExpression.ParseLambda(funcParameters, typeof(object), formattedExpression);
            Delegate compiledExpression = expression.Compile();
            //TODO: This leaves much to be desired
            Expression<Func<object[], object>> methodFunc = @params => compiledExpression.DynamicInvoke(@params);

            return methodFunc.Compile();
        }
    }
}
