using Newtonsoft.Json;
using Serilog;

namespace Web.Utils.Serialization.Serializers
{
    public static class ApiControllerSerializers
    {       
        public static JsonSerializerSettings ConfigureForApiControllers(this JsonSerializerSettings settings, ILogger logger) =>
            settings
                .ConfigureForApi()
                .ConfigureErrorLogger(logger.ForContext<JsonSerializerSettings>());
    }
}
