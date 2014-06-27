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
        private static readonly IDictionary<MethodSignature, Func<object[], object>> _methods = new Dictionary<MethodSignature, Func<object[], object>>();

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
                var signature = new MethodSignature(conditionFormat, parameters.Select(x => x != null ? x.GetType() : typeof(object)).ToArray());
                Func<object[], object> method;
                if (_methods.TryGetValue(signature, out method) == false)
                {
                    _methods[signature] = method = GenerateMethod(conditionFormat, parameters);
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
                .Select(x => Expression.Parameter(parameters[x] != null ? parameters[x].GetType() : typeof(object),
                    string.Format("param{0}", x))).ToArray();

            LambdaExpression expression = DynamicExpression.ParseLambda(funcParameters, typeof(object), formattedExpression);
            ParameterExpression initalParameter = Expression.Parameter(typeof(object[]));
            var foo = Enumerable.Range(0, parameters.Length)
                    .Select(x => Expression.ConvertChecked(Expression.ArrayIndex(initalParameter, Expression.Constant(x)),
                                parameters[x] != null ? parameters[x].GetType() : typeof(object)));
            var invokeExp = Expression.Invoke(expression, foo);
            var end = Expression.Lambda<Func<object[], object>>(invokeExp, initalParameter);
            return end.Compile();
        }

        private class MethodSignature
        {
            public MethodSignature(string conditionFormat, IList<Type> parameterTypes)
            {
                ConditionFormat = conditionFormat;
                ParameterTypes = parameterTypes;
            }

            private string ConditionFormat { get; set; }
            private IList<Type> ParameterTypes { get; set; }

            private bool Equals(MethodSignature other)
            {
                if (string.Equals(ConditionFormat, other.ConditionFormat))
                {
                    if (ReferenceEquals(ParameterTypes, other.ParameterTypes))
                        return true;
                    if (ParameterTypes != null && other.ParameterTypes != null &&
                        ParameterTypes.Count == other.ParameterTypes.Count)
                    {
                        return ParameterTypes.Where((t, i) => t != other.ParameterTypes[i]).Any() == false;
                    }
                }

                return false;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((MethodSignature)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int rv = (ConditionFormat != null ? ConditionFormat.GetHashCode() : 0) * 397;
                    if (ParameterTypes != null)
                    {
                        rv = ParameterTypes.Aggregate(rv, (current, type) => current ^ type.GetHashCode());
                    }
                    return rv;
                }
            }
        }
    }
}
