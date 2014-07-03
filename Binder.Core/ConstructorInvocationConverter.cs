using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace Binder.Core
{
    public abstract class ConstructorInvocationConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        protected object CreateObject(object[] constructorParameters, object targetType)
        {
            var objectType = targetType as Type;
            if (objectType == null)
            {
                var typeString = targetType as string;
                if (string.IsNullOrWhiteSpace(typeString) == false)
                    objectType = Type.GetType(typeString, false, true);
            }

            if (objectType == null)
                return DependencyProperty.UnsetValue;

            if (constructorParameters == null)
                constructorParameters = new object[0];

            ConstructorInfo ctorInfo =
                objectType.GetConstructors()
                    .SingleOrDefault(x => x.GetParameters().Length == constructorParameters.Length);
            
            if (ctorInfo == null)
            {
                Debug.WriteLine("{0}: Could not find constructor for type '{1}'", GetType().FullName, objectType.FullName);
                return DependencyProperty.UnsetValue;
            }
            try
            {
                return ctorInfo.Invoke(constructorParameters);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} - EXCEPTION: {1}", GetType().FullName, ex);
                return DependencyProperty.UnsetValue;
            }
        }

    }
}