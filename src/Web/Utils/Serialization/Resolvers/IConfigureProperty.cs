using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.Utils.Serialization.Resolvers
{
    public interface IConfigureProperty
    {
        void ConfigureProperty(JsonProperty property, PropertyInfo propertyInfo, JsonSerializerSettings serializeSettings);
    }
}
