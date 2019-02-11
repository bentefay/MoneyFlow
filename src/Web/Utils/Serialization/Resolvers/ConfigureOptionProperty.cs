using System.Reflection;
using LanguageExt;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.Utils.Serialization.Resolvers
{
    public class ConfigureOptionProperty: IConfigureProperty
    {
        public void ConfigureProperty(JsonProperty property, PropertyInfo propertyInfo, JsonSerializerSettings serializeSettings)
        {
            if (IsAnOptionType(propertyInfo))
            {
                var typeInsideOption = propertyInfo.PropertyType.GetGenericArguments()[0];

                var shouldSerializeMethod = typeof(ConfigureOptionProperty)
                    .GetMethod(nameof(ConfigureOptionProperty.ShouldSerialize), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeInsideOption);

                bool ShouldSerialize(object value) => (bool)shouldSerializeMethod.Invoke(null, new [] { value });

                property.ShouldSerialize =
                    instance =>
                        serializeSettings.NullValueHandling == NullValueHandling.Include ||
                        property.NullValueHandling == NullValueHandling.Include ||
                        ShouldSerialize(propertyInfo.GetValue(instance));
            }
        }

        private static bool IsAnOptionType(PropertyInfo propertyInfo) =>
            propertyInfo.PropertyType.IsGenericType &&
            propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Option<>);

        private static bool ShouldSerialize<T>(Option<T> option) => option.IsSome;
    }
}
