using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using Web.Utils.Serialization.Converters;
using Web.Utils.Serialization.Resolvers;

namespace Web.Utils.Serialization
{
    public static class JsonSerializerSettingsConfiguration
    {
        public static JsonSerializerSettings ConfigureResolverWithCamelCase(this JsonSerializerSettings settings)
        {
            settings.ContractResolver = new ContractResolver(settings, ConfigurePropertyConverters.All)
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            return settings;
        }

        public static JsonSerializerSettings ConfigureTinyTypes(this JsonSerializerSettings settings)
        {
            settings.Converters.Add(new TinyTypeJsonConverter());
            return settings;
        }

        public static JsonSerializerSettings ConfigureErrorLogger(
            this JsonSerializerSettings settings,
            ILogger logger
        )
        {
            settings.Error = (sender, args) =>
            {
                var errorContext = args?.ErrorContext;
                string Nls(int n) => new string('\n', n);
                string ValueOrPlaceholder(object value) => value?.ToString() ?? "<value not present>";

                var message =
                    $"Json.net threw an error during serialization/deserialization.{Nls(2)}" +
                    $"Path: {{Path}}{Nls(1)}" +
                    $"Member: {{Member}}{Nls(1)}" +
                    $"OriginalObject Type: {{OriginalObjectType}}{Nls(1)}" +
                    $"CurrentObject Type: {{CurrentObjectType}}{Nls(2)}" +
                    $"Exception Message: {{ErrorMessage}}{Nls(1)}";

                logger.Error(
                    message,
                    ValueOrPlaceholder(errorContext?.Path),
                    ValueOrPlaceholder(errorContext?.Member),
                    ValueOrPlaceholder(errorContext?.OriginalObject?.GetType()),
                    ValueOrPlaceholder(args?.CurrentObject?.GetType()),
                    ValueOrPlaceholder(errorContext?.Error?.Message)
                );

                if (args?.ErrorContext?.Handled != null)
                {
                    args.ErrorContext.Handled = true;
                }
            };
            return settings;
        }

        public static JsonSerializerSettings IgnoreNulls(this JsonSerializerSettings settings)
        {
            settings.NullValueHandling = NullValueHandling.Ignore;
            return settings;
        }

        public static JsonSerializerSettings ConfigureEnumsAsString(this JsonSerializerSettings settings)
        {
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }

        public static JsonSerializerSettings ConfigureSumTypes(this JsonSerializerSettings settings)
        {
            return settings;
        }

        public static JsonSerializerSettings ConfigureExceptionConverter(this JsonSerializerSettings settings)
        {
            settings.Converters.Add(new ExceptionConverter());
            return settings;
        }
    }
}
