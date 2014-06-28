using System;
using System.Collections.Generic;
using System.Linq;

namespace Binder.Core
{
    internal class MethodSignature
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
                    int index = 1;
                    rv = ParameterTypes.Aggregate(rv, (current, type) => current ^ (type.GetHashCode() * index++ * 397));
                }
                return rv;
            }
        }
    }
}