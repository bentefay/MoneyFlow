using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Web.Utils.Extensions
{
    public static class TypeExtensions
    {
        public static string GetNameWithoutGenericAnnotations(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var name = type.Name;
            return name.Substring(0, name.IndexOf('`'));
        }

        public static bool IsAssignableTo(this Type @this, Type type) => type.IsAssignableFrom(@this);

        public static bool ImplementsOrExtends(this Type @this, Type type) => @this.IsAssignableTo(type);

        public static bool ImplementsOrExtendsWithOpenTypes(this Type @this, Type type)
        {
            if (@this.ImplementsOrExtends(type))
                return true;

            if (type.IsGenericType)
            {
                var openType = type.IsConstructedGenericType ?
                    type.GetGenericTypeDefinition() :
                    type;

                return @this
                    .GetParentTypesIncludingSelf()
                    .Where(t => t.IsGenericType)
                    .Any(t => t.GetGenericTypeDefinition() == openType);
            }
            else
            {
                return false;
            }
        }

        public static IEnumerable<T> GetCustomAttributesInherited<T>(this Type type) where T : Attribute =>
            type.GetCustomAttributes<T>(inherit: true)
                .Union(type.GetInterfaces().SelectMany(i => i.GetCustomAttributes<T>(inherit: true)));

        public static IEnumerable<Type> GetParentTypesIncludingSelf(this Type type) =>
            type.GetParentTypesIncludingSelfAtLeastOnce().Distinct();

        public static IEnumerable<Type> GetParentTypesIncludingSelfAtLeastOnce(this Type type)
        {
            var searchTypes = new Queue<Type>();

            searchTypes.Enqueue(type);

            while (searchTypes.Count > 0)
            {
                var parentType = searchTypes.Dequeue();

                yield return parentType;

                if (parentType.BaseType != null)
                {
                    searchTypes.Enqueue(parentType.BaseType);
                }

                foreach (var i in parentType.GetInterfaces())
                {
                    searchTypes.Enqueue(i);
                }
            }
        }

        public static Type CreateExampleConcreteTypeFromOpenGeneric(this Type type)
        {
            if (!type.ContainsGenericParameters)
                return type;

            var concreteArguments = type
                .GetGenericArguments()
                .Select(genericArgument =>
                {
                    var genericConstraint = genericArgument.GetGenericParameterConstraints().FirstOrDefault();

                    if (genericConstraint == null)
                        return typeof(object);

                    return genericConstraint == typeof(ValueType) ? typeof(int) : genericConstraint;
                })
                .ToArray();

            return type.MakeGenericType(concreteArguments);
        }
    }
}
