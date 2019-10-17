using Newtonsoft.Json;

namespace Web.Utils.Serialization.Serializers
{
    public static class ApiSerializers
    {
        public static readonly JsonSerializerSettings Instance = new JsonSerializerSettings().ConfigureForApi();

        public static JsonSerializerSettings ConfigureForApi(this JsonSerializerSettings settings) =>
            settings
                .ConfigureResolverWithCamelCase()
                .ConfigureTinyTypes()
                .ConfigureSumTypes()
                .ConfigureExceptionConverter()
                .ConfigureEnumsAsString();
    }
}
