using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace Binder.Core
{
    public static class MethodManager
    {
        private static readonly IDictionary<MethodSignature, Func<object[], object>> _methods =
            new Dictionary<MethodSignature, Func<object[], object>>();

        public static bool IsCached(string conditionFormat, IList<Type> parameterTypes)
        {
            var signature = new MethodSignature(conditionFormat, parameterTypes);
            return _methods.ContainsKey(signature);
        }

        public static void ClearCache()
        {
            lock (_methods)
            {
                _methods.Clear();
            }
        }

        public static bool Remove(string conditionFormat, IList<Type> parameterTypes)
        {
            lock (_methods)
            {
                var signature = new MethodSignature(conditionFormat, parameterTypes);
                return _methods.Remove(signature);
            }
        }

        public static object RunMethod(string conditionFormat, object[] parameters)
        {
            lock (_methods)
            {
                Type[] parameterTypes = parameters.Select(x => x != null ? x.GetType() : typeof(object)).ToArray();
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