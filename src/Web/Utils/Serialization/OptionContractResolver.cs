using System.Reflection;
using LanguageExt;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.Utils.Serialization
{
    public class OptionContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly NullValueHandling _nullValueHandling;

        public OptionContractResolver(NullValueHandling nullValueHandling)
        {
            _nullValueHandling = nullValueHandling;
        }
        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (member is PropertyInfo propertyInfo && IsAnOptionType(propertyInfo))
            {
                var typeInOption = propertyInfo.PropertyType.GetGenericArguments()[0];

                var isSomeMethod = typeof(OptionContractResolver)
                    .GetMethod(nameof(IsSome), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeInOption);
                               
                property.ShouldSerialize =
                    instance => 
                        _nullValueHandling == NullValueHandling.Include ||
                        property.NullValueHandling == NullValueHandling.Include || 
                        ExecuteIsSome(isSomeMethod, propertyInfo.GetValue(instance));
            }

            return property;
        }

        private static bool IsAnOptionType(PropertyInfo propInfo) => 
            propInfo.PropertyType.IsGenericType &&
            propInfo.PropertyType.GetGenericTypeDefinition() == typeof(Option<>);

        private static bool ExecuteIsSome(MethodInfo isSome, object value) => (bool)isSome.Invoke(null, new [] { value });
        
        private static bool IsSome<T>(Option<T> option) => option.IsSome;
    }
}