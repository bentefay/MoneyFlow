using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Web.Types;
using Web.Utils.Serialization.Converters;

namespace Web.Utils.Serialization.Resolvers
{
    public class ConfigureTinyTypeProperty: IConfigureProperty
    {
        public void ConfigureProperty(JsonProperty property, PropertyInfo propertyInfo, JsonSerializerSettings serializeSettings)
        {
            if (IsATinyType(propertyInfo))
            {
                var typeInsideTinyType = TinyTypeJsonConverter.GetValuePropertyType(propertyInfo.PropertyType)
                    .Match(type => type, error => throw new Exception($"This should not be possible. Error: {error}"));

                var shouldSerializeMethod = typeof(ConfigureTinyTypeProperty)
                    .GetMethod(nameof(ConfigureTinyTypeProperty.ShouldSerialize), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeInsideTinyType);

                bool ShouldSerialize(object value) => (bool)shouldSerializeMethod.Invoke(null, new [] { value });

                property.ShouldSerialize =
                    instance =>
                        serializeSettings.NullValueHandling == NullValueHandling.Include ||
                        property.NullValueHandling == NullValueHandling.Include ||
                        ShouldSerialize(propertyInfo.GetValue(instance));
            }
        }

        private static bool IsATinyType(PropertyInfo propInfo) =>
            TinyTypeJsonConverter.ImplementsTinyTypeInterface(propInfo.PropertyType);

        private static bool ShouldSerialize<T>(ITinyType<T> tinyType) => !ReferenceEquals(null, tinyType.Value);
    }
}
