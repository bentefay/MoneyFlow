using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Web.Utils.Serialization
{
    public static class JsonSerializerSettingsConfiguration
    {
        public static JsonSerializerSettings ConfigureErrorOnMissingMember(this JsonSerializerSettings @this)
        {
            @this.MissingMemberHandling = MissingMemberHandling.Error;
            return @this;
        }
        
        public static JsonSerializerSettings ConfigureCamelCase(this JsonSerializerSettings @this)
        {
            @this.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            return @this;
        }

        public static JsonSerializerSettings ConfigureErrorLogger(
            this JsonSerializerSettings @this,
            ILogger logger
        )
        {
            @this.Error = (sender, args) =>
            {
                var errorContext = args?.ErrorContext;
                string Nls(int n) => new string('\n', n);
                string ValueOrPlaceholder(object? value) => value?.ToString() ?? "<value not present>";

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
            return @this;
        }

        public static JsonSerializerSettings ConfigureEnumsAsString(this JsonSerializerSettings settings)
        {
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }
    }
}
